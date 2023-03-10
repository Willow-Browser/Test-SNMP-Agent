using System.Runtime.Serialization;

namespace Engine.Pipeline
{
    /// <summary>
    /// Access failure exception. 
    /// Raised when,
    /// 1. GET operation is performed on a write-only object.
    /// 2. SET operation is performed on a read-only object.
    /// </summary>
    [DataContract]
    public sealed class AccessFailureException : Exception
    {
        /// <summary>
        /// Creates a <see cref="AccessFailureException"/>.
        /// </summary>
        public AccessFailureException()
        {

        }

        /// <summary>
        /// Creates a <see cref="AccessFailureException"/> instance with a specific <see cref="String"/>.
        /// </summary>
        /// <param name="message">Message</param>
        public AccessFailureException(string message) : base(message)
        {

        }

        /// <summary>
        /// Creates a <see cref="AccessFailureException"/> instance with a specific <see cref="String"/> and an <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner exception</param>
        public AccessFailureException(string message, Exception inner) : base(message, inner)
        {

        }

        /// <summary>
        /// Creates a <see cref="AccessFailureException"/> instance.
        /// </summary>
        /// <param name="info">Info</param>
        /// <param name="context">Context</param>
        private AccessFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents this <see cref="AccessFailureException"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "AccessFailureException: " + Message;
        }
    }
}