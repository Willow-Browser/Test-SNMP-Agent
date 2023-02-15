using System.Net.NetworkInformation;
using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// ifInNUcastPkts object.
    /// </summary>
    internal sealed class IfInNUcastPkts : ScalarObject
    {
        private readonly NetworkInterface networkInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfInNUcastPkts"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="networkInterface">The network interface.</param>
        public IfInNUcastPkts(int index, NetworkInterface networkInterface)
            : base("1.3.6.1.2.1.2.2.1.12.{0}", index)
        {
            this.networkInterface = networkInterface;
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
            get { return new Counter32(networkInterface.GetIPStatistics().NonUnicastPacketsReceived); }
            set { throw new AccessFailureException(); }
        }
    }
}