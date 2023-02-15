namespace Engine.Pipeline
{
    /// <summary>
    /// SNMP application class, who is a pipeline for message processing.
    /// </summary>
    public sealed class SnmpApplication
    {
        private readonly ILogger logger;
        private readonly IMembershipProvider provider;
        private readonly MessageHandlerFactory factory;
        private readonly ObjectStore store;
        private readonly SnmpApplicationFactory owner;
        private IMessageHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpApplication"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="store">The store.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="factory">The factory.</param>
        public SnmpApplication(SnmpApplicationFactory owner, ILogger logger, ObjectStore store, IMembershipProvider provider, MessageHandlerFactory factory)
        {
            this.owner = owner;
            this.provider = provider;
            this.logger = logger;
            this.store = store;
            this.factory = factory;
        }

        /// <summary>
        /// Initializes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Init(ISnmpContext context)
        {
            Context = context;
            ProcessingFinished = false;
            handler = null;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public ISnmpContext Context { get; private set; }

        /// <summary>
        /// Gets a value indicating whether processing is finished.
        /// </summary>
        /// <value><c>true</c> if processing is finished; otherwise, <c>false</c>.</value>
        public bool ProcessingFinished { get; private set; }

        /// <summary>
        /// Processes an incoming request.
        /// </summary>
        public void Process()
        {
            OnAuthenticateRequest();

            // TODO: add authorization.
            OnMapRequestHandler();
            OnRequestHandlerExecute();
            OnLogRequest();
            owner.Reuse(this);
        }

        private void OnRequestHandlerExecute()
        {
            if (ProcessingFinished)
            {
                return;
            }

            handler.Handle(Context, store);
        }

        private void OnMapRequestHandler()
        {
            if (ProcessingFinished)
            {
                return;
            }

            handler = factory.GetHandler(Context.Request);
            if (handler is NullMessageHandler)
            {
                // TODO: handle error here.
                CompleteProcessing();
            }
        }

        private void OnAuthenticateRequest()
        {
            if (!provider.AuthenticateRequest(Context))
            {
                // TODO: handle error here.
                // return TRAP saying authenticationFailed.
                CompleteProcessing();
            }

            if (Context.Response != null)
            {
                CompleteProcessing();
            }
        }

        private void OnLogRequest()
        {
            Context.SendResponse();
            if (logger == null)
            {
                return;
            }

            logger.Log(Context);
        }

        /// <summary>
        /// Completes the processing.
        /// </summary>
        public void CompleteProcessing()
        {
            ProcessingFinished = true;
        }
    }
}