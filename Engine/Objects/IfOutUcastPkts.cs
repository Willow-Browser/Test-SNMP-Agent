using System.Net.NetworkInformation;
using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// ifOutUcastPkts.
    /// </summary>
    internal sealed class IfOutUcastPkts : ScalarObject
    {
        private readonly NetworkInterface networkInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfOutUcastPkts"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="networkInterface">The network interface.</param>
        public IfOutUcastPkts(int index, NetworkInterface networkInterface)
            : base("1.3.6.1.2.1.2.2.1.17.{0}", index)
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
            get { return new Counter32(networkInterface.GetIPStatistics().UnicastPacketsSent); }
            set { throw new AccessFailureException(); }
        }
    }
}