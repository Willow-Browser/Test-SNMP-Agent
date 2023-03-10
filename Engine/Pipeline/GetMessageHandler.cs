using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// GET message handler.
    /// </summary>
    /// <remarks>
    /// Follows RFC 3416 4.2.1.
    /// </remarks>
    public sealed class GetMessageHandler : IMessageHandler
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="store">The object store.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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

            var index = 0;
            IList<Variable> result = new List<Variable>();
            foreach (var v in context.Request.Pdu().Variables)
            {
                index++;
                try
                {
                    var obj = store.GetObject(v.Id);
                    if (obj == null)
                    {
                        result.Add(new Variable(v.Id, new NoSuchInstance()));
                    }
                    else
                    {
                        var item = obj.Variable;
                        result.Add(item);
                    }
                }
                catch (AccessFailureException)
                {
                    result.Add(new Variable(v.Id, new NoSuchObject()));
                }
                catch (Exception)
                {
                    context.CopyRequest(ErrorCode.GenError, index);
                    return;
                }
            }

            context.GenerateResponse(result);
        }
    }
}