using System.Net.NetworkInformation;
using Engine.Pipeline;

namespace Engine.Objects
{
    /// <summary>
    /// ifTable object.
    /// </summary>
    public sealed class IfTable : TableObject
    {
        // "1.3.6.1.2.1.2.2"
        private readonly IList<ScalarObject> _elements = new List<ScalarObject>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IfTable"/> class.
        /// </summary>
        public IfTable()
        {
            NetworkChange.NetworkAddressChanged +=
                (sender, args) => LoadElements();

            NetworkChange.NetworkAvailabilityChanged +=
                (sender, args) => LoadElements();

            LoadElements();
        }

        private void LoadElements()
        {
            _elements.Clear();
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var columnTypes = new[]
                {
                    typeof(IfIndex),
                    typeof(IfDescr),
                    typeof(IfType),
                    typeof(IfMtu),
                    typeof(IfSpeed),
                    typeof(IfPhysAddress),
                    typeof(IfAdminStatus),
                    typeof(IfOperStatus),
                    typeof(IfLastChange),
                    typeof(IfInOctets),
                    typeof(IfInUcastPkts),
                    typeof(IfInNUcastPkts),
                    typeof(IfInDiscards),
                    typeof(IfInErrors),
                    typeof(IfInUnknownProtos),
                    typeof(IfOutOctets),
                    typeof(IfOutUcastPkts),
                    typeof(IfOutNUcastPkts),
                    typeof(IfOutDiscards),
                    typeof(IfOutErrors),
                    typeof(IfOutQLen),
                    typeof(IfSpecific)
                };
            foreach (var type in columnTypes)
            {
                for (int i = 0; i < interfaces.Length; i++)
                {
                    _elements.Add((ScalarObject)Activator.CreateInstance(type, new object[] { i + 1, interfaces[i] }));
                }
            }
        }

        /// <summary>
        /// Gets the objects in the table.
        /// </summary>
        /// <value>
        /// The objects.
        /// </value>
        protected override IEnumerable<ScalarObject> Objects
        {
            get { return _elements; }
        }
    }
}