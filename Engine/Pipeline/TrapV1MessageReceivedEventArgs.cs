using System.Net;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// Trap v1 message received event args.
    /// </summary>
    public sealed class TrapV1MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrapV1MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="request">The request.</param>
        /// <param name="binding">The binding.</param>
        public TrapV1MessageReceivedEventArgs(IPEndPoint sender, TrapV1Message request, IListenerBinding binding)
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
            TrapV1Message = request;
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
        public TrapV1Message TrapV1Message { get; private set; }

        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        /// <value>The binding.</value>
        public IListenerBinding Binding { get; private set; }
    }
}