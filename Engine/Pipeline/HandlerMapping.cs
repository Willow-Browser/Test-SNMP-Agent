using System.Diagnostics.CodeAnalysis;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// Handler mapping class, who is used to map incoming messages to their handlers.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public sealed class HandlerMapping
    {
        private readonly string[] version;
        private readonly bool catchAll;
        private readonly string command;
        private readonly IMessageHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerMapping"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler.</param>
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1027:TabsMustNotBeUsed", Justification = "Reviewed. Suppression is OK here.")]
        public HandlerMapping(string version, string command, IMessageHandler handler)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            catchAll = version == "*";
            this.version = catchAll ?
                new string[0] :
                version.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                ;
            this.command = command;
            this.handler = handler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerMapping"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="command">The command.</param>
        /// <param name="type">The type.</param>
        /// <param name="assembly">The assembly.</param>
        public HandlerMapping(string version, string command, string type, string assembly)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            catchAll = version == "*";
            this.version = catchAll ?
                new string[0] :
                version.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                ;
            this.command = command;
            handler = CreateMessageHandler(assembly, type);
        }

        private static IMessageHandler CreateMessageHandler(string assemblyName, string type)
        {
            foreach (var assembly in from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     let name = assembly.GetName().Name
                                     where string.Compare(name, assemblyName, StringComparison.OrdinalIgnoreCase) == 0
                                     select assembly)
            {
                return (IMessageHandler)Activator.CreateInstance(assembly.GetType(type));
            }

            return (IMessageHandler)Activator.CreateInstance(AppDomain.CurrentDomain.Load(assemblyName).GetType(type));
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <value>The handler.</value>
        public IMessageHandler Handler
        {
            get { return handler; }
        }

        /// <summary>
        /// Determines whether this instance can handle the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///     <c>true</c> if this instance can handle the specified message; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(ISnmpMessage message)
        {
            return VersionMatched(message) && CommandMatched(message);
        }

        private bool CommandMatched(ISnmpMessage message)
        {
            var codeString = message.Pdu().TypeCode.ToString();
            return StringEquals(command, "*") || StringEquals(command + "RequestPdu", codeString) ||
            StringEquals(command + "Pdu", codeString);
        }

        private bool VersionMatched(ISnmpMessage message)
        {
            return catchAll || version.Any(v => StringEquals(message.Version.ToString(), v));
        }

        private static bool StringEquals(string left, string right)
        {
            return string.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}