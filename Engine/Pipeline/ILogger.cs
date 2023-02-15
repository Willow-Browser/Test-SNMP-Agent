namespace Engine.Pipeline
{
    /// <summary>
    /// Logger interface.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="context">Message context.</param>
        void Log(ISnmpContext context);
    }
}