using System.Net.NetworkInformation;
using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// ifMtu object.
    /// </summary>
    internal sealed class IfMtu : ScalarObject
    {
        private readonly ISnmpData data;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfMtu"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="networkInterface">The network interface.</param>
        public IfMtu(int index, NetworkInterface networkInterface)
            : base("1.3.6.1.2.1.2.2.1.4.{0}", index)
        {
            if (networkInterface.Supports(NetworkInterfaceComponent.IPv4))
            {
                var pv4InterfaceProperties = networkInterface.GetIPProperties().GetIPv4Properties();
                data = new Integer32(pv4InterfaceProperties == null ? -1 : pv4InterfaceProperties.Mtu);
            }
            else
            {
                try
                {
                    data = new Integer32(networkInterface.GetIPProperties().GetIPv6Properties().Mtu);
                }
                catch (NotImplementedException)
                {
                    data = new Integer32(0);
                }
            }
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