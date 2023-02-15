namespace Engine.Pipeline
{
    using Lextm.SharpSnmpLib;
    using Lextm.SharpSnmpLib.Messaging;

    /// <summary>
    /// SNMP version 3 membership provider. Not yet implemented.
    /// </summary>    
    public sealed class Version3MembershipProvider : IMembershipProvider
    {
        private const VersionCode Version = VersionCode.V3;

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

            return context.Request.Version == Version && context.HandleMembership();
        }
    }
}