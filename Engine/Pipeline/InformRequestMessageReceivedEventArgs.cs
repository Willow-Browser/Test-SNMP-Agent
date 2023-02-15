using System.Net;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// Inform request message received event args.
    /// </summary>
    public sealed class InformRequestMessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InformRequestMessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="request">The request.</param>
        /// <param name="binding">The binding.</param>
        public InformRequestMessageReceivedEventArgs(IPEndPoint sender, InformRequestMessage request, IListenerBinding binding)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (binding == null)
            {
                throw new ArgumentNullException(nameof(binding));
            }

            Sender = sender;
            InformRequestMessage = request;
            Binding = binding;
        }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        /// <value>The sender.</value>
        public IPEndPoint Sender { get; private set; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        public InformRequestMessage InformRequestMessage { get; private set; }

        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        /// <value>The binding.</value>
        public IListenerBinding Binding { get; private set; }
    }
}