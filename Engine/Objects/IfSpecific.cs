using System.Net.NetworkInformation;
using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// ifSpecific.
    /// </summary>
    internal sealed class IfSpecific : ScalarObject
    {
        private readonly ISnmpData data;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfSpecific"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="networkInterface">The network interface.</param>
        public IfSpecific(int index, NetworkInterface networkInterface)
            : base("1.3.6.1.2.1.2.2.1.22.{0}", index)
        {
            data = new ObjectIdentifier("0.0");
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        /// <exception cref="AccessFailureException"></exception>
        public override ISnmpData Data
        {
            get { return data; }
            set { throw new AccessFailureException(); }
        }
    }
}