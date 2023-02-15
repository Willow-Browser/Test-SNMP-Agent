using System.Net.NetworkInformation;
using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// ifType.
    /// </summary>
    internal sealed class IfType : ScalarObject
    {
        private readonly ISnmpData data;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfType"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="networkInterface">The network interface.</param>
        public IfType(int index, NetworkInterface networkInterface)
            : base("1.3.6.1.2.1.2.2.1.3.{0}", index)
        {
            data = new Integer32((int)networkInterface.NetworkInterfaceType);
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