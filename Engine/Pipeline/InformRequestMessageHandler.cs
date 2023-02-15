using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// Message handler for INFORM.
    /// </summary>    
    public sealed class InformRequestMessageHandler : IMessageHandler
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

            InvokeMessageReceived(new InformRequestMessageReceivedEventArgs(context.Sender, (InformRequestMessage)context.Request, context.Binding));
            context.CopyRequest(ErrorCode.NoError, 0);
        }

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        public event EventHandler<InformRequestMessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Invokes the message received.
        /// </summary>
        /// <param name="e">The <see cref="Samples.Pipeline.InformRequestMessageReceivedEventArgs"/> instance containing the event data.</param>
        private void InvokeMessageReceived(InformRequestMessageReceivedEventArgs e)
        {
            var handler = MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}