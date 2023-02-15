using Lextm.SharpSnmpLib;

namespace Engine.Pipeline
{
    /// <summary>
    /// SNMP object store, who holds all implemented SNMP objects in the agent.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public class ObjectStore
    {
        /// <summary>The internal list of objects holding the data.</summary>
        protected readonly IList<ISnmpObject> List = new List<ISnmpObject>();

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="id">The object id.</param>
        /// <returns></returns>
        public virtual ScalarObject? GetObject(ObjectIdentifier id)
        {
            return List.Select(o => o.MatchGet(id)).FirstOrDefault(result => result != null);
        }

        /// <summary>
        /// Gets the next object.
        /// </summary>
        /// <param name="id">The object id.</param>
        /// <returns></returns>
        public virtual ScalarObject? GetNextObject(ObjectIdentifier id)
        {
            return List.Select(o => o.MatchGetNext(id)).FirstOrDefault(result => result != null);
        }

        /// <summary>
        /// Adds the specified <see cref="ISnmpObject"/>.
        /// </summary>
        /// <param name="newObject">The object.</param>
        public virtual void Add(ISnmpObject newObject)
        {
            List.Add(newObject);
        }
    }
}