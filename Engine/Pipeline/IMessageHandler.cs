namespace Engine.Pipeline
{
    /// <summary>
    /// Message handler interface.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="store">The object store.</param>
        /// <returns></returns>
        void Handle(ISnmpContext context, ObjectStore store);
    }
}