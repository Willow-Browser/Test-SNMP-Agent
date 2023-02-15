using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// sysUpTime object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "UpTime")]
    public sealed class SysUpTime : ScalarObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SysUpTime"/> class.
        /// </summary>
        public SysUpTime()
            : base(new ObjectIdentifier("1.3.6.1.2.1.1.3.0"))
        {
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public override ISnmpData Data
        {
            get { return new TimeTicks((uint)Environment.TickCount / 10); }
            set { throw new AccessFailureException(); }
        }
    }
}