using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// SET message handler.
    /// </summary>    
    /// <remarks>
    /// Follows RFC 3416 4.2.1.
    /// Not supported scenarios are, 
    ///   wrongLength, 
    ///   wrongEncoding, 
    ///   wrongValue, 
    ///   noCreation,
    ///   inconsistentName,
    ///   inconsistentValue,
    ///   resourceUnavailable,
    ///   commitFailed,
    ///   undoFailed
    /// </remarks>
    public sealed class SetMessageHandler : IMessageHandler
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

            context.CopyRequest(ErrorCode.InconsistentName, int.MaxValue);
            if (context.TooBig)
            {
                context.GenerateTooBig();
                return;
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
                        status = ErrorCode.NoAccess;
                    }
                    catch (ArgumentException ex)
                    {
                        if (!Enum.TryParse<ErrorCode>(ex.Message, out status) || status == ErrorCode.NoError)
                            status = ErrorCode.WrongType;
                    }
                    catch (Exception)
                    {
                        status = ErrorCode.GenError;
                    }
                }
                else
                {
                    status = ErrorCode.NotWritable;
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