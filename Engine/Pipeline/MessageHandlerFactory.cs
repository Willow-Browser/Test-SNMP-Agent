using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// Message handler factory.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public sealed class MessageHandlerFactory
    {
        private readonly HandlerMapping[] mappings;
        private readonly NullMessageHandler nullHandler = new NullMessageHandler();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerFactory"/> class.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        public MessageHandlerFactory(HandlerMapping[] mappings)
        {
            if (mappings == null)
            {
                throw new ArgumentNullException(nameof(mappings));
            }

            this.mappings = mappings;
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public IMessageHandler GetHandler(ISnmpMessage message)
        {
            foreach (var mapping in mappings.Where(mapping => mapping.CanHandle(message)))
            {
                return mapping.Handler;
            }

            return nullHandler;
        }
    }
}