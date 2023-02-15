using System.Net;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// Listener binding interface.
    /// </summary>
    public interface IListenerBinding
    {
        /// <summary>
        /// Sends a response message.
        /// </summary>
        /// <param name="response">
        /// A <see cref="ISnmpMessage"/>.
        /// </param>
        /// <param name="receiver">Receiver.</param>
        void SendResponse(ISnmpMessage snmpMessage, EndPoint receiver);

        /// <summary>
        /// Sends a response message.
        /// </summary>
        /// <param name="response">
        /// A <see cref="ISnmpMessage"/>.
        /// </param>
        /// <param name="receiver">Receiver.</param>
        Task SendResponseAsync(ISnmpMessage snmpMessage, EndPoint receiver);

        /// <summary>
        /// Endpoint.
        /// </summary>
        IPEndPoint EndPoint { get; }
    }
}