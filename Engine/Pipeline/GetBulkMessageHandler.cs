using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// GET BULK message handler.
    /// </summary>  
    public sealed class GetBulkMessageHandler : IMessageHandler
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="store">The object store.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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

            var pdu = context.Request.Pdu();
            IList<Variable> result = new List<Variable>();
            var index = 0;
            var nonrepeaters = pdu.ErrorStatus.ToInt32();
            var variables = pdu.Variables;
            for (var i = 0; i < nonrepeaters; i++)
            {
                var v = variables[i];
                index++;
                try
                {
                    var next = store.GetNextObject(v.Id);
                    result.Add(next == null ? new Variable(v.Id, new EndOfMibView()) : next.Variable);
                }
                catch (Exception)
                {
                    context.CopyRequest(ErrorCode.GenError, index);
                    return;
                }
            }

            for (var j = nonrepeaters; j < variables.Count; j++)
            {
                var v = variables[j];
                index++;
                var temp = v;
                var repetition = pdu.ErrorIndex.ToInt32();
                while (repetition-- > 0)
                {
                    try
                    {
                        var next = store.GetNextObject(temp.Id);
                        if (next == null)
                        {
                            temp = new Variable(temp.Id, new EndOfMibView());
                            result.Add(temp);
                            break;
                        }

                        // TODO: how to handle write only object here?
                        result.Add(next.Variable);
                        temp = next.Variable;
                    }
                    catch (Exception)
                    {
                        context.CopyRequest(ErrorCode.GenError, index);
                        return;
                    }
                }
            }

            context.GenerateResponse(result);
        }
    }
}