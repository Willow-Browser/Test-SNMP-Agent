using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;

namespace Engine.Pipeline
{
    /// <summary>
    /// SNMP context factory.
    /// </summary>s
    public static class SnmpContextFactory
    {
        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="users">The users.</param>
        /// <param name="group">The engine group.</param>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        public static ISnmpContext Create(ISnmpMessage request, IPEndPoint sender, UserRegistry users, EngineGroup group, IListenerBinding binding)
        {
            if (request.Version == VersionCode.V3)
            {
                return new SecureSnmpContext(request, sender, users, group, binding);
            }

            return new NormalSnmpContext(request, sender, users, binding);
        }
    }
}