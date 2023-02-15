using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// SET message handler.
    /// </summary>    
    /// <remarks>
    /// Follows RFC 1157, 4.1.5
    /// </remarks>
    public sealed class SetV1MessageHandler : IMessageHandler
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
            var status = ErrorCode.NoError;

            IList<Variable> result = new List<Variable>();
            foreach (var v in context.Request.Pdu().Variables)
            {
                index++;
                var obj = store.GetObject(v.Id);
                if (obj != null)
                {
                    try
                    {
                        obj.Data = v.Data;
                    }
                    catch (AccessFailureException)
                    {
                        status = ErrorCode.NoSuchName;
                    }
                    catch (ArgumentException)
                    {
                        status = ErrorCode.BadValue;
                    }
                    catch (Exception)
                    {
                        status = ErrorCode.GenError;
                    }
                }
                else
                {
                    status = ErrorCode.NoSuchName;
                }

                if (status != ErrorCode.NoError)
                {
                    context.CopyRequest(status, index);
                    return;
                }

                result.Add(v);
            }

            context.GenerateResponse(result);
        }
    }
}