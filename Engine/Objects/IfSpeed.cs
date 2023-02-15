using System.Net.NetworkInformation;
using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// ifSpeed.
    /// </summary>
    internal sealed class IfSpeed : ScalarObject
    {
        private readonly NetworkInterface networkInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfSpeed"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="networkInterface">The network interface.</param>
        public IfSpeed(int index, NetworkInterface networkInterface)
            : base("1.3.6.1.2.1.2.2.1.5.{0}", index)
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
                    return new Gauge32(networkInterface.Speed);
                }
                catch (PlatformNotSupportedException)
                {
                    return new Gauge32(0);
                }
            }

            set
            { throw new AccessFailureException(); }
        }
    }
}