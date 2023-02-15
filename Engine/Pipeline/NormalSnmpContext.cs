using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;

namespace Engine.Pipeline
{
    /// <summary>
    /// Normal SNMP context class. It is v1 and v2c specific.
    /// </summary>
    public sealed class NormalSnmpContext : SnmpContextBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NormalSnmpContext"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="users">The users.</param>
        /// <param name="binding">The binding.</param>
        public NormalSnmpContext(ISnmpMessage request, IPEndPoint sender, UserRegistry users, IListenerBinding binding)
            : base(request, sender, users, null, binding)
        {
        }

        /// <summary>
        /// Copies the request variable bindings to response.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="index">The index.</param>
        public override void CopyRequest(ErrorCode status, int index)
        {
            Response = new ResponseMessage(
                Request.RequestId(),
                Request.Version,
                Request.Parameters.UserName,
                status,
                index,
                DecoratedVariables
            );
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
            Response = new ResponseMessage(
                Request.RequestId(),
                Request.Version,
                Request.Parameters.UserName,
                ErrorCode.TooBig,
                0,
                Request.Pdu().Variables);
        }

        /// <summary>
        /// Handles the membership.
        /// </summary>
        /// <returns>Always returns <c>false</c>.</returns>
        public override bool HandleMembership()
        {
            return false;
        }

        /// <summary>
        /// Generates the response.
        /// </summary>
        /// <param name="variables">The variables.</param>
        public override void GenerateResponse(IList<Variable> variables)
        {
            Response = new ResponseMessage(
                Request.RequestId(),
                Request.Version,
                Request.Parameters.UserName,
                ErrorCode.NoError,
                0,
                variables);
            if (TooBig)
            {
                GenerateTooBig();
            }
        }
    }
}