using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// SNMP engine, who is the core of an SNMP entity (manager or agent).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public sealed class SnmpEngine : IDisposable
    {
        private readonly SnmpApplicationFactory factory;
        private readonly EngineGroup group;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpEngine"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="group">Engine core group.</param>
        public SnmpEngine(SnmpApplicationFactory factory, Listener listener, EngineGroup group)
        {
            this.factory = factory;
            Listener = listener;
            this.group = group;
        }

        /// <summary>
        /// Disposes resources in use.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SnmpEngine"/> is reclaimed by garbage collection.
        /// </summary>
        ~SnmpEngine()
        {
            Dispose(false);
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

            if (disposing)
            {
                if (Listener != null)
                {
                    Listener.Dispose();
                    Listener = null;
                }
            }

            disposed = true;
        }

        /// <summary>
        /// Gets or sets the listener.
        /// </summary>
        /// <value>The listener.</value>
        public Listener Listener { get; private set; }

        private void ListenerMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var request = e.Message;
            var context = SnmpContextFactory.Create(request, e.Sender, Listener.Users, group, e.Binding);
            var application = factory.Create(context);
            application.Process();
        }

        /// <summary>
        /// Starts the engine.
        /// </summary>
        public void Start()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            Listener.ExceptionRaised += ListenerExceptionRaised;
            Listener.MessageReceived += ListenerMessageReceived;
            Listener.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            Listener.Stop();
            Listener.ExceptionRaised -= ListenerExceptionRaised;
            Listener.MessageReceived -= ListenerMessageReceived;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SnmpEngine"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                return Listener.Active;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void ListenerExceptionRaised(object sender, ExceptionRaisedEventArgs e)
        {
            var handler = ExceptionRaised;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Occurs when an exception is raised.
        /// </summary>
        public event EventHandler<ExceptionRaisedEventArgs> ExceptionRaised;
    }
}