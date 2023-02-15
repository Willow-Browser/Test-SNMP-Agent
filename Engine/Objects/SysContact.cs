using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// sysContact object.
    /// </summary>
    public sealed class SysContact : ScalarObject
    {
        private OctetString contact = new OctetString(Environment.UserName);

        /// <summary>
        /// Initializes a new instance of the <see cref="SysContact"/> class.
        /// </summary>
        public SysContact() : base(new ObjectIdentifier("1.3.6.1.2.1.1.4.0"))
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
                return contact;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                // TODO: should we allow Null?
                if (value.TypeCode != SnmpType.OctetString)
                {
                    throw new ArgumentException("Invalid data type.", nameof(value));
                }
                if (((OctetString)value).ToString().Length > 255) //respect DisplayString syntax length limitation
                {
                    throw new ArgumentException(nameof(ErrorCode.WrongLength));
                }

                contact = (OctetString)value;
            }
        }
    }
}