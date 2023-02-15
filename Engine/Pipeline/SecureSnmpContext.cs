using System.Configuration;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;

namespace Engine.Pipeline
{
    /// <summary>
    /// Secure SNMP context. It is specific to v3.
    /// </summary>
    internal sealed class SecureSnmpContext : SnmpContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecureSnmpContext"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="users">The users.</param>
        /// <param name="group">The engine core group.</param>
        /// <param name="binding">The binding.</param>
        public SecureSnmpContext(ISnmpMessage request, IPEndPoint sender, UserRegistry users, EngineGroup group, IListenerBinding binding)
            : base(request, sender, users, group, binding)
        {
        }

        private void HandleFailure(Variable failure)
        {
            var defaultPair = DefaultPrivacyProvider.DefaultPair;
            var time = Group.EngineTimeData;
            Response = new ReportMessage(
                Request.Version,
                new Header(
                    new Integer32(Request.MessageId()),
                    new Integer32(Messenger.MaxMessageSize),
                    0), // no need to encrypt.
                new SecurityParameters(
                    Group.EngineId,
                    new Integer32(time[0]),
                    new Integer32(time[1]),
                    Request.Parameters.UserName,
                    defaultPair.AuthenticationProvider.CleanDigest,
                    defaultPair.Salt),
                new Scope(
                    Request.Scope?.ContextEngineId ?? OctetString.Empty,
                    Request.Scope?.ContextName ?? OctetString.Empty,
                    new ReportPdu(
                        Request.RequestId(),
                        ErrorCode.NoError,
                        0,
                        new List<Variable>(1) { failure })),
                defaultPair,
                null);
            if (TooBig)
            {
                GenerateTooBig();
            }
        }

        public override void CopyRequest(ErrorCode status, int index)
        {
            var userName = Request.Parameters.UserName;
            var privacy = Users.Find(userName);
            var time = Group.EngineTimeData;
            Response = new ResponseMessage(
                    Request.Version,
                    new Header(
                        new Integer32(Request.MessageId()),
                        new Integer32(Messenger.MaxMessageSize),
                        privacy.ToSecurityLevel()),
                    new SecurityParameters(
                        Group.EngineId,
                        new Integer32(time[0]),
                        new Integer32(time[1]),
                        userName,
                        privacy.AuthenticationProvider.CleanDigest,
                        privacy.Salt),
                    new Scope(
                        Request.Scope.ContextEngineId,
                        Request.Scope.ContextName,
                        new ResponsePdu(
                            Request.RequestId(),
                            status,
                            index,
                            DecoratedVariables)),
                    privacy,
                    true,
                    null);
            if (TooBig)
            {
                GenerateTooBig();
            }
        }

        /// <summary>
        /// Generates too big message.
        /// </summary>
        public override void GenerateTooBig()
        {
            var userName = Request.Parameters.UserName;
            var privacy = Users.Find(userName);
            var time = Group.EngineTimeData;
            Response = new ResponseMessage(
                Request.Version,
                new Header(
                    new Integer32(Request.MessageId()),
                    new Integer32(Messenger.MaxMessageSize),
                    privacy.ToSecurityLevel()),
                new SecurityParameters(
                    Group.EngineId,
                    new Integer32(time[0]),
                    new Integer32(time[1]),
                    userName,
                    privacy.AuthenticationProvider.CleanDigest,
                    privacy.Salt),
                new Scope(
                    Request.Scope.ContextEngineId,
                    Request.Scope.ContextName,
                    new ResponsePdu(
                        Request.RequestId(),
                        ErrorCode.TooBig,
                        0,
                        Request.Pdu().Variables)),
                privacy,
                true,
                null);
            if (TooBig)
            {
                Response = null;

                // TODO: snmpSilentDrops++;
            }
        }

        /// <summary>
        /// Handles the membership.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public override bool HandleMembership()
        {
            var request = Request;
            var parameters = request.Parameters;
            var typeCode = Request.TypeCode();

            if (typeCode == SnmpType.Unknown)
            {
                HandleFailure(Group.DecryptionError);
                return false;
            }

            if (typeCode != SnmpType.TrapV2Pdu && parameters.EngineId != Group.EngineId)
            {
                HandleDiscovery();
                return true;
            }

            var user = Users.Find(parameters.UserName);
            if (user == null)
            {
                HandleFailure(Group.UnknownSecurityName);
                return false;
            }

            if (typeCode == SnmpType.TrapV2Pdu && (user.EngineIds == null || !user.EngineIds.Contains(parameters.EngineId)))
            {
                HandleFailure(Group.UnknownEngineId);
                return false;
            }

            if (parameters.IsInvalid)
            {
                HandleFailure(Group.AuthenticationFailure);
                return false;
            }

            if (typeCode == SnmpType.TrapV2Pdu)
            {
                return true;
            }

            if ((user.ToSecurityLevel() | Levels.Reportable) != request.Header.SecurityLevel)
            {
                HandleFailure(Group.UnsupportedSecurityLevel);
                return false;
            }

            var inTime = EngineGroup.IsInTime(Group.EngineTimeData, parameters.EngineBoots.ToInt32(), parameters.EngineTime.ToInt32());
            if (!inTime)
            {
                HandleFailure(Group.NotInTimeWindow);
                return false;
            }

            return true;
        }

        private static bool? timeIncluded;

        private static bool TimeIncluded
        {
            get
            {
                if (timeIncluded == null)
                {
                    timeIncluded = true;
                }

                return timeIncluded.Value;
            }
        }

        private void HandleDiscovery()
        {
            // discovery message received.
            var time = Group.EngineTimeData;
            Response = new ReportMessage(
                VersionCode.V3,
                new Header(
                    new Integer32(Request.MessageId()),
                    new Integer32(Messenger.MaxMessageSize),
                    0), // no need to encrypt for discovery.
                new SecurityParameters(
                    Group.EngineId,
                    TimeIncluded ? new Integer32(time[0]) : Integer32.Zero,
                    TimeIncluded ? new Integer32(time[1]) : Integer32.Zero,
                    OctetString.Empty,
                    OctetString.Empty,
                    OctetString.Empty),
                new Scope(
                    Request.Scope.ContextEngineId,
                    Request.Scope.ContextName,
                    new ReportPdu(
                        Request.RequestId(),
                        ErrorCode.NoError,
                        0,
                        new List<Variable>(1) { Group.UnknownEngineId })),
                DefaultPrivacyProvider.DefaultPair,
                null);
            if (TooBig)
            {
                GenerateTooBig();
            }
        }

        /// <summary>
        /// Generates the response.
        /// </summary>
        /// <param name="variables">The variables.</param>
        public override void GenerateResponse(IList<Variable> variables)
        {
            var userName = Request.Parameters.UserName;
            var privacy = Users.Find(userName);
            var time = Group.EngineTimeData;
            Response = new ResponseMessage(
                Request.Version,
                new Header(
                    new Integer32(Request.MessageId()),
                    new Integer32(Messenger.MaxMessageSize),
                    privacy.ToSecurityLevel()),
                new SecurityParameters(
                    Group.EngineId,
                    new Integer32(time[0]),
                    new Integer32(time[1]),
                    userName,
                    privacy.AuthenticationProvider.CleanDigest,
                    privacy.Salt),
                new Scope(
                    Request.Scope.ContextEngineId,
                    Request.Scope.ContextName,
                    new ResponsePdu(
                        Request.RequestId(),
                        ErrorCode.NoError,
                        0,
                        variables)),
                privacy,
                true,
                null);
            if (TooBig)
            {
                GenerateTooBig();
            }
        }
    }
}