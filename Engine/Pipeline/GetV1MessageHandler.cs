using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// GET message handler.
    /// </summary>
    /// <remarks>
    /// Follows RFC 1157, 4.1.2
    /// </remarks>
    public sealed class GetV1MessageHandler : IMessageHandler
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

            var status = ErrorCode.NoError;
            var index = 0;
            IList<Variable> result = new List<Variable>();
            foreach (var v in context.Request.Pdu().Variables)
            {
                index++;
                var obj = store.GetObject(v.Id);
                if (obj != null)
                {
                    try
                    {
                        var item = obj.Variable;
                        result.Add(item);
                    }
                    catch (AccessFailureException)
                    {
                        status = ErrorCode.NoSuchName;
                    }
                    catch (Exception)
                    {
                        context.CopyRequest(ErrorCode.GenError, index);
                        return;
                    }
                }
                else
                {
                    status = ErrorCode.NoSuchName;
                }

                if (status == ErrorCode.NoError)
                {
                    continue;
                }

                context.CopyRequest(status, index);
                return;
            }

            context.GenerateResponse(result);
        }
    }
}