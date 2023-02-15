using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// sysName object.
    /// </summary>
    public sealed class SysName : ScalarObject
    {
        private OctetString name = new OctetString(Environment.MachineName);

        /// <summary>
        /// Initializes a new instance of the <see cref="SysName"/> class.
        /// </summary>
        public SysName()
            : base(new ObjectIdentifier("1.3.6.1.2.1.1.5.0"))
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
                return name;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value.TypeCode != SnmpType.OctetString)
                {
                    throw new ArgumentException("Invalid data type.", nameof(value));
                }

                name = (OctetString)value;
            }
        }
    }
}