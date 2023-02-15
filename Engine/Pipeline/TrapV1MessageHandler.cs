using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// Message handler for TRAP v1.
    /// </summary>    
    public sealed class TrapV1MessageHandler : IMessageHandler
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="store">The object store.</param>
        /// <returns></returns>
        public void Handle(ISnmpContext context, ObjectStore store)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            InvokeMessageReceived(new TrapV1MessageReceivedEventArgs(context.Sender, (TrapV1Message)context.Request, context.Binding));
        }

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        public event EventHandler<TrapV1MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Invokes the message received event handler.
        /// </summary>
        /// <param name="e">The <see cref="Lextm.SharpSnmpLib.Messaging.MessageReceivedEventArgs"/> instance containing the event data.</param>
        private void InvokeMessageReceived(TrapV1MessageReceivedEventArgs e)
        {
            var handler = MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}