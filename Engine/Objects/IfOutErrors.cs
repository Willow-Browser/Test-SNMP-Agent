using System.Net.NetworkInformation;
using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// ifOutErrors object.
    /// </summary>
    internal sealed class IfOutErrors : ScalarObject
    {
        private readonly NetworkInterface networkInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfOutErrors"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="networkInterface">The network interface.</param>
        public IfOutErrors(int index, NetworkInterface networkInterface)
            : base("1.3.6.1.2.1.2.2.1.20.{0}", index)
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
            get { return new Counter32(networkInterface.GetIPStatistics().OutgoingPacketsWithErrors); }
            set { throw new AccessFailureException(); }
        }
    }
}