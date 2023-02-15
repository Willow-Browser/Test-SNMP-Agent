using System.Net;
using System.Net.Sockets;

namespace Engine.Pipeline
{
    using Lextm.SharpSnmpLib.Messaging;
    using Lextm.SharpSnmpLib.Security;

    /// <summary>
    /// Binding class for <see cref="Listener"/>.
    /// </summary>
    public sealed class ListenerBinding : IDisposable, IListenerBinding
    {
        private readonly UserRegistry users;
        private Socket? socket;
        private int bufferSize;
        private int active; // = Inactive
        private bool disposed;
        private const int Active = 1;
        private const int Inactive = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerBinding"/> class.
        /// </summary>
        /// <param name="users">The users.</param>
        /// <param name="endpoint">The endpoint.</param>
        public ListenerBinding(UserRegistry users, IPEndPoint endPoint)
        {
            this.users = users;
            EndPoint = endPoint;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            if (disposing)
            {
                active = Inactive;
                if (socket != null)
                {
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both); // Note that closing the socket releases the _socket.ReceiveFrom call.
                    }
                    catch (SocketException ex)
                    {
                        // This exception is thrown in .NET Core <=2.1.4 on non-Windows systems.
                        // However, the shutdown call is necessary to release the socket binding.
                        if (!SnmpMessageExtension.IsRunningOnWindows && ex.SocketErrorCode == SocketError.NotConnected)
                        {

                        }
                    }

                    socket.Dispose();
                    socket = null;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Listener"/> is reclaimed by garbage collection.
        /// </summary>
        ~ListenerBinding()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Events
        /// <summary>
        /// Occurs when an exception is raised.
        /// </summary>
        /// <remarks>The exception can be both <see cref="SocketException"/> and <see cref="SnmpException"/>.</remarks>
        public event EventHandler<ExceptionRaisedEventArgs> ExceptionRaised;

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        #endregion

        /// <summary>
        /// Sends a response message.
        /// </summary>
        /// <param name="response">
        /// A <see cref="ISnmpMessage"/>.
        /// </param>
        /// <param name="receiver">Receiver.</param>
        public void SendResponse(ISnmpMessage response, EndPoint receiver)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (receiver == null)
            {
                throw new ArgumentNullException(nameof(receiver));
            }

            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (socket == null)
            {
                return;
            }

            var buffer = response.ToBytes();

            try
            {
                socket.SendTo(buffer, 0, buffer.Length, 0, receiver);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.Interrupted)
                {
                    // IMPORTANT: interrupted means the socket is closed.
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>The endpoint.</value>
        public IPEndPoint EndPoint { get; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="PortInUseException"/>
        public void Start()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            var addressFamily = EndPoint.AddressFamily;
            if (addressFamily == AddressFamily.InterNetwork && !Socket.OSSupportsIPv4)
            {
                throw new InvalidOperationException(Listener.ErrorIPv4NotSupported);
            }

            if (addressFamily == AddressFamily.InterNetworkV6 && !Socket.OSSupportsIPv6)
            {
                throw new InvalidOperationException(Listener.ErrorIPv6NotSupported);
            }

            var activeBefore = Interlocked.CompareExchange(ref active, Active, Inactive);
            if (activeBefore == Active)
            {
                // already started. Nothing to do
                return;
            }

            socket = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);
            if (SnmpMessageExtension.IsRunningOnWindows)
            {
                socket.ExclusiveAddressUse = true;
            }

            try
            {
                socket.Bind(EndPoint);
            }
            catch (SocketException ex)
            {
                Interlocked.Exchange(ref active, Inactive);
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    throw new PortInUseException("Endpoint is already in use", ex)
                    {
                        Endpoint = EndPoint
                    };
                }

                throw;
            }

            bufferSize = socket.ReceiveBufferSize = Messenger.MaxMessageSize;

#if ASYNC
            Task.Factory.StartNew(() => AsyncBeginReceive());
#else
            Task.Factory.StartNew(() => AsyncReceive());
#endif
        }

        /// <summary>
        /// Stops.
        /// </summary>
        /// <exception cref="ObjectDisposedException"/>
        public void Stop()
        {
            if (disposed || socket == null)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            var activeBefore = Interlocked.CompareExchange(ref active, Inactive, Active);
            if (activeBefore != Active)
            {
                return;
            }

            try
            {
                socket.Shutdown(SocketShutdown.Both); // Note that closing the socket releases the _socket.ReceiveFrom call.
            }
            catch (SocketException ex)
            {
                // This exception is thrown in .NET Core <=2.1.4 on non-Windows systems.
                // However, the shutdown call is necessary to release the socket binding.
                if (!SnmpMessageExtension.IsRunningOnWindows && ex.SocketErrorCode == SocketError.NotConnected)
                {

                }
            }

            socket.Dispose();
            socket = null;
        }

#if ASYNC
        private void AsyncBeginReceive()
        {
            while (true)
            {
                // If no more active, then stop.
                if (Interlocked.Exchange(ref active, active) == Inactive)
                {
                    return;
                }

                byte[] buffer = new byte[bufferSize];
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                IAsyncResult iar = null;
                try
                {
                    iar = socket.BeginReceiveFrom(buffer, 0, bufferSize, SocketFlags.None, ref remote, AsyncEndReceive, buffer);
                }
                catch (SocketException ex)
                {
                    // ignore WSAECONNRESET, http://bytes.com/topic/c-sharp/answers/237558-strange-udp-socket-problem
                    if (ex.SocketErrorCode != SocketError.ConnectionReset)
                    {
                        // If the SnmpTrapListener was active, marks it as stopped and call HandleException.
                        // If it was inactive, the exception is likely to result from this, and we raise nothing.
                        long activeBefore = Interlocked.CompareExchange(ref active, Inactive, Active);
                        if (activeBefore == Active)
                        {
                            HandleException(ex);
                        }
                    }
                }

                if (iar != null)
                {
                    iar.AsyncWaitHandle.WaitOne();
                }
            }
        }

        private void AsyncEndReceive(IAsyncResult iar)
        {
            // If no more active, then stop. This discards the received packet, if any (indeed, we may be there either
            // because we've received a packet, or because the socket has been closed).
            if (Interlocked.Exchange(ref active, active) == Inactive)
            {
                return;
            }

            //// We start another receive operation.
            //AsyncBeginReceive();

            byte[] buffer = (byte[])iar.AsyncState;

            try
            {
                EndPoint remote = socket.AddressFamily == AddressFamily.InterNetwork ? new IPEndPoint(IPAddress.Any, 0) : new IPEndPoint(IPAddress.IPv6Any, 0);
                int count = socket.EndReceiveFrom(iar, ref remote);
                HandleMessage(buffer, count, (IPEndPoint)remote);
            }
            catch (SocketException ex)
            {
                // ignore WSAECONNRESET, http://bytes.com/topic/c-sharp/answers/237558-strange-udp-socket-problem
                if (ex.SocketErrorCode != SocketError.ConnectionReset)
                {
                    // If the SnmpTrapListener was active, marks it as stopped and call HandleException.
                    // If it was inactive, the exception is likely to result from this, and we raise nothing.
                    long activeBefore = Interlocked.CompareExchange(ref active, Inactive, Active);
                    if (activeBefore == Active)
                    {
                        HandleException(ex);
                    }
                }
            }
        }
#else
        private void AsyncReceive()
        {
            while (true)
            {
                if (Interlocked.Exchange(ref active, active) == Inactive)
                {
                    return;
                }

                if (socket == null)
                {
                    return;
                }

                try
                {
                    var buffer = new byte[bufferSize];
                    EndPoint remote = socket.AddressFamily == AddressFamily.InterNetwork ? new IPEndPoint(IPAddress.Any, 0) : new IPEndPoint(IPAddress.IPv6Any, 0);

                    var count = socket.ReceiveFrom(buffer, ref remote);
                    Task.Factory.StartNew(() => HandleMessage(buffer, count, (IPEndPoint)remote));
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.ConnectionReset)
                    {
                        var activeBefore = Interlocked.CompareExchange(ref active, Inactive, Active);
                        if (activeBefore == Active)
                        {
                            HandleException(ex);
                        }
                    }
                }
            }
        }
#endif

        private void HandleException(Exception exception)
        {
            ExceptionRaised.Invoke(this, new ExceptionRaisedEventArgs(exception));
        }

        private void HandleMessage(byte[] buffer, int count, IPEndPoint remote)
        {
            IList<ISnmpMessage>? messages = null;

            try
            {
                messages = MessageFactory.ParseMessages(buffer, 0, count, users);
            }
            catch (Exception ex)
            {
                var exception = new MessageFactoryException("Invalid message bytes found. Use tracing to analyze the bytes.", ex);
                exception.SetBytes(buffer);
                HandleException(exception);
            }

            if (messages == null)
            {
                return;
            }

            foreach (var message in messages)
            {
                var handler = MessageReceived;
                handler.Invoke(this, new MessageReceivedEventArgs(remote, message, this));
            }
        }

        /// <summary>
        /// Sends a response message.
        /// </summary>
        /// <param name="response">
        /// A <see cref="ISnmpMessage"/>.
        /// </param>
        /// <param name="receiver">Receiver.</param>
        public async Task SendResponseAsync(ISnmpMessage response, EndPoint receiver)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (receiver == null)
            {
                throw new ArgumentNullException(nameof(receiver));
            }

            if (disposed)
            {
                throw new ObjectDisposedException("Listener");
            }

            if (socket == null)
            {
                return;
            }

            try
            {
                await socket.SendToAsync(new ArraySegment<byte>(response.ToBytes()), SocketFlags.None, receiver);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.Interrupted)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="PortInUseException"/>
        public async Task StartAsync()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            var addressFamily = EndPoint.AddressFamily;
            if (addressFamily == AddressFamily.InterNetwork && !Socket.OSSupportsIPv4)
            {
                throw new InvalidOperationException(Listener.ErrorIPv4NotSupported);
            }

            if (addressFamily == AddressFamily.InterNetworkV6 && !Socket.OSSupportsIPv6)
            {
                throw new InvalidOperationException(Listener.ErrorIPv6NotSupported);
            }

            var activeBefore = Interlocked.CompareExchange(ref active, Active, Inactive);
            if (activeBefore == Active)
            {
                // already started. Nothing to do
                return;
            }

            socket = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);
            if (SnmpMessageExtension.IsRunningOnWindows)
            {
                socket.ExclusiveAddressUse = true;
            }

            try
            {
                socket.Bind(EndPoint);
            }
            catch (SocketException ex)
            {
                Interlocked.Exchange(ref active, Inactive);
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    throw new PortInUseException("Endpoint is already in use", ex)
                    {
                        Endpoint = EndPoint
                    };
                }

                throw;
            }

            bufferSize = socket.ReceiveBufferSize;
            await ReceiveAsync();
        }

        private async Task ReceiveAsync()
        {
            while (true)
            {
                if (Interlocked.Exchange(ref active, active) == Inactive)
                {
                    return;
                }

                if (socket == null)
                {
                    return;
                }

                int count;
                var reply = new byte[bufferSize];
                try
                {
                    EndPoint remote = socket.AddressFamily == AddressFamily.InterNetwork ? new IPEndPoint(IPAddress.Any, 0) : new IPEndPoint(IPAddress.IPv6Any, 0);
                    var result = await socket.ReceiveMessageFromAsync(new ArraySegment<byte>(reply), SocketFlags.None, remote);
                    count = result.ReceivedBytes;
                    await Task.Factory.StartNew(() => HandleMessage(reply, count, (IPEndPoint)result.RemoteEndPoint));
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.ConnectionReset)
                    {
                        var activeBefore = Interlocked.CompareExchange(ref active, Inactive, Active);
                        if (activeBefore == Active)
                        {
                            HandleException(ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents a <see cref="Listener"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ListenerBinding";
        }
    }
}