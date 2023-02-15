using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// sysORTable object.
    /// </summary>
    public sealed class SysORTable : TableObject
    {
        // "1.3.6.1.2.1.1.9.1"
        private readonly IList<ScalarObject> elements = new List<ScalarObject>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SysORTable"/> class.
        /// </summary>
        public SysORTable()
        {
            elements.Add(new SysORIndex(1));
            elements.Add(new SysORIndex(2));
            elements.Add(new SysORID(1, new ObjectIdentifier("1.3")));
            elements.Add(new SysORID(2, new ObjectIdentifier("1.4")));
            elements.Add(new SysORDescr(1, new OctetString("Test1")));
            elements.Add(new SysORDescr(2, new OctetString("Test2")));
            elements.Add(new SysORUpTime(1, new TimeTicks(1)));
            elements.Add(new SysORUpTime(2, new TimeTicks(2)));
        }

        /// <summary>
        /// Gets the objects in the table.
        /// </summary>
        /// <value>The objects.</value>
        protected override IEnumerable<ScalarObject> Objects
        {
            get { return elements; }
        }
    }
}