using System.Net.NetworkInformation;
using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// ifIndex object.
    /// </summary>
    internal sealed class IfIndex : ScalarObject
    {
        private readonly ISnmpData data;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfIndex"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="networkInterface">The network interface.</param>
        public IfIndex(int index, NetworkInterface networkInterface)
            : base("1.3.6.1.2.1.2.2.1.1.{0}", index)
        {
            data = new Integer32(index);
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