using System.Net;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// Trap v2 message received event args.
    /// </summary>
    public sealed class TrapV2MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrapV2MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="request">The request.</param>
        /// <param name="binding">The binding.</param>
        public TrapV2MessageReceivedEventArgs(IPEndPoint sender, TrapV2Message request, IListenerBinding binding)
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
            TrapV2Message = request;
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
        public TrapV2Message TrapV2Message { get; private set; }

        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        /// <value>The binding.</value>
        public IListenerBinding Binding { get; private set; }
    }
}