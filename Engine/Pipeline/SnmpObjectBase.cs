using Lextm.SharpSnmpLib;

namespace Engine.Pipeline
{
    /// <summary>
    /// SNMP basic object.
    /// </summary>
    public abstract class SnmpObjectBase : ISnmpObject
    {
        /// <summary>
        /// Matches the GET NEXT criteria.
        /// </summary>
        /// <param name="id">The ID in GET NEXT message.</param>
        /// <returns><c>null</c> if it does not match.</returns>
        public abstract ScalarObject? MatchGetNext(ObjectIdentifier id);

        /// <summary>
        /// Matches the GET criteria.
        /// </summary>
        /// <param name="id">The ID in GET message.</param>
        /// <returns><c>null</c> if it does not match.</returns>
        public abstract ScalarObject? MatchGet(ObjectIdentifier id);
    }
}