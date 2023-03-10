using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// sysObjectId object.
    /// </summary>
    public sealed class SysObjectId : ScalarObject
    {
        private readonly ObjectIdentifier objectId = new ObjectIdentifier("1.3.6.1");

        /// <summary>
        /// Initializes a new instance of the <see cref="SysObjectId"/> class.
        /// </summary>
        public SysObjectId()
            : base(new ObjectIdentifier("1.3.6.1.2.1.1.2.0"))
        {
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public override ISnmpData Data
        {
            get { return objectId; }
            set { throw new AccessFailureException(); }
        }
    }
}