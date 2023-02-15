using System.Linq;

namespace Engine.Pipeline
{
    /// <summary>
    /// Composed membership provider, who owns internal providers. If the request is authenticated by any of the internal providers, it is considered as authenticated.
    /// </summary>  
    public sealed class ComposedMembershipProvider : IMembershipProvider
    {
        private readonly IMembershipProvider[] providers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposedMembershipProvider"/> class.
        /// </summary>
        /// <param name="providers">The internal providers.</param>
        public ComposedMembershipProvider(IMembershipProvider[] providers)
        {
            this.providers = providers;
        }

        /// <summary>
        /// Authenticates the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public bool AuthenticateRequest(ISnmpContext context)
        {
            return providers.Any(provider => provider.AuthenticateRequest(context));
        }
    }
}