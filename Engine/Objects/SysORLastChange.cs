using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// sysORLastChange object.
    /// </summary>
    public sealed class SysORLastChange : ScalarObject
    {
        private readonly ISnmpData value = new TimeTicks(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="SysORLastChange"/> class.
        /// </summary>
        public SysORLastChange()
            : base(new ObjectIdentifier("1.3.6.1.2.1.1.8.0"))
        {
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public override ISnmpData Data
        {
            get { return value; }
            set { throw new AccessFailureException(); }
        }
    }
}