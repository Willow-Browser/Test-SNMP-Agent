using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// SNMP version 1 membership provider, who checks community names for security.
    /// </summary>    
    public sealed class Version1MembershipProvider : IMembershipProvider
    {
        private const VersionCode Version = VersionCode.V1;
        private readonly OctetString get;
        private readonly OctetString set;

        /// <summary>
        /// Initializes a new instance of the <see cref="Version1MembershipProvider"/> class.
        /// </summary>
        /// <param name="getCommunity">The get community.</param>
        /// <param name="setCommunity">The set community.</param>
        public Version1MembershipProvider(OctetString getCommunity, OctetString setCommunity)
        {
            get = getCommunity;
            set = setCommunity;
        }

        /// <summary>
        /// Authenticates the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public bool AuthenticateRequest(ISnmpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            var request = context.Request;
            if (request.Version != Version)
            {
                return false;
            }
            
            var parameters = request.Parameters;
            if (request.Pdu().TypeCode == SnmpType.SetRequestPdu)
            {
                return parameters.UserName == set;
            }

            return parameters.UserName == get;
        }
    }
}