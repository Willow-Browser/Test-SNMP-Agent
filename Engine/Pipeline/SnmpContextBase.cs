using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;

namespace Engine.Pipeline
{
    /// <summary>
    /// SNMP context.
    /// </summary>
    public abstract class SnmpContextBase : ISnmpContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpContextBase"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="users">The users.</param>
        /// <param name="group">The engine core group.</param>
        /// <param name="binding">The binding.</param>
        protected SnmpContextBase(ISnmpMessage request, IPEndPoint sender, UserRegistry users, EngineGroup group, IListenerBinding binding)
        {
            Request = request;
            Binding = binding;
            Users = users;
            Sender = sender;
            CreatedTime = DateTime.Now;
            Group = group;
        }

        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        /// <value>The binding.</value>
        public IListenerBinding Binding { get; private set; }

        /// <summary>
        /// Gets the created time.
        /// </summary>
        /// <value>The created time.</value>
        public DateTime CreatedTime { get; private set; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>The request.</value>
        public ISnmpMessage Request { get; private set; }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The users.</value>
        protected UserRegistry Users { get; private set; }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>The response.</value>
        public ISnmpMessage? Response { get; protected set; }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        /// <value>The sender.</value>
        public IPEndPoint Sender { get; private set; }

        /// <summary>
        /// Gets or sets the objects.
        /// </summary>
        /// <value>The objects.</value>
        protected EngineGroup Group { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [too big].
        /// </summary>
        /// <value><c>true</c> if the response is too big; otherwise, <c>false</c>.</value>
        public bool TooBig
        {
            get
            {
                if (Response == null)
                {
                    return false;
                }

                var length = Response.ToBytes().Length;
                return length > Request.Header.MaxSize || length > Messenger.MaxMessageSize;
            }
        }

        /// <summary>
        /// Sends out response message.
        /// </summary>
        public void SendResponse()
        {
            if (Response == null)
            {
                return;
            }

            Binding.SendResponse(Response, Sender);
        }

        /// <summary>
        /// Generates the response.
        /// </summary>
        /// <param name="variables">The variables.</param>
        public abstract void GenerateResponse(IList<Variable> variables);

        /// <summary>
        /// Copies the request variable bindings to response.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="index">The index.</param>
        public abstract void CopyRequest(ErrorCode status, int index);

        /// <summary>
        /// Handles the membership authentication.
        /// </summary>
        /// <returns></returns>
        public abstract bool HandleMembership();

        /// <summary>
        /// Generates too big message.
        /// </summary>
        public abstract void GenerateTooBig();

        protected IList<Variable> DecoratedVariables
        {
            get
            {
                if (Request.TypeCode() == SnmpType.TrapV2Pdu)
                {
                    var pdu = (TrapV2Pdu)Request.Pdu();
                    return pdu.Decorate(Request.Variables());
                }
                else if (Request.TypeCode() == SnmpType.InformRequestPdu)
                {
                    var pdu = (InformRequestPdu)Request.Pdu();
                    return pdu.Decorate(Request.Variables());
                }

                return Request.Variables();
            }
        }
    }
}