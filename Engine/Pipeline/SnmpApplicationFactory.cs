namespace Engine.Pipeline
{
    /// <summary>
    /// SNMP application factory, who holds all pipeline instances.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public sealed class SnmpApplicationFactory
    {
        private readonly ILogger logger;
        private readonly ObjectStore store;
        private readonly IMembershipProvider membershipProvider;
        private readonly MessageHandlerFactory factory;
        private readonly object root = new object();
        private readonly Queue<SnmpApplication> queue = new Queue<SnmpApplication>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpApplicationFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The store.</param>
        /// <param name="membershipProvider">The membership provider.</param>
        /// <param name="factory">The factory.</param>
        public SnmpApplicationFactory(ILogger logger, ObjectStore store, IMembershipProvider membershipProvider, MessageHandlerFactory factory)
        {
            this.logger = logger;
            this.membershipProvider = membershipProvider;
            this.store = store;
            this.factory = factory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpApplicationFactory"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="membershipProvider">The membership provider.</param>
        /// <param name="factory">The factory.</param>
        public SnmpApplicationFactory(ObjectStore store, IMembershipProvider membershipProvider, MessageHandlerFactory factory)
            : this(null, store, membershipProvider, factory) // TODO: handle the null case in the future.
        {
        }

        /// <summary>
        /// Creates a pipeline for the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public SnmpApplication Create(ISnmpContext context)
        {
            SnmpApplication result = null;
            lock (root)
            {
                if (queue.Count > 0)
                {
                    result = queue.Dequeue();
                }
            }

            if (result == null)
            {
                result = new SnmpApplication(this, logger, store, membershipProvider, factory);
            }

            result.Init(context);
            return result;
        }

        /// <summary>
        /// Reuses the specified pipeline.
        /// </summary>
        /// <param name="application">The application.</param>
        internal void Reuse(SnmpApplication application)
        {
            lock (root)
            {
                queue.Enqueue(application);
            }
        }
    }
}