using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// sysLocation object.
    /// </summary>
    public sealed class SysLocation : ScalarObject
    {
        private OctetString location = OctetString.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="SysLocation"/> class.
        /// </summary>
        public SysLocation()
            : base(new ObjectIdentifier("1.3.6.1.2.1.1.6.0"))
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
                return location; 
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

                location = (OctetString)value;
            }
        }
    }
}