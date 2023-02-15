using System.Net.NetworkInformation;
using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// ifNumber object.
    /// </summary>
    public sealed class IfNumber : ScalarObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IfNumber"/> class.
        /// </summary>
        public IfNumber()
            : base(new ObjectIdentifier("1.3.6.1.2.1.2.1.0"))
        {
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public override ISnmpData Data
        {
            get
            {
                return new Integer32(NetworkInterface.GetAllNetworkInterfaces().Length);
            }

            set
            {
                throw new AccessFailureException();
            }
        }
    }
}