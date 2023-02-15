using System.Net.NetworkInformation;
using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// ifInUnknownProtos object.
    /// </summary>
    internal sealed class IfInUnknownProtos : ScalarObject
    {
        private readonly NetworkInterface networkInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfInUnknownProtos"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="networkInterface">The network interface.</param>
        public IfInUnknownProtos(int index, NetworkInterface networkInterface)
            : base("1.3.6.1.2.1.2.2.1.15.{0}", index)
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
            get
            {
                try
                {
                    return new Counter32(networkInterface.GetIPStatistics().IncomingUnknownProtocolPackets);
                }
                catch (PlatformNotSupportedException)
                {
                    return new Counter32(0);
                }
            }

            set
            { throw new AccessFailureException(); }
        }
    }
}