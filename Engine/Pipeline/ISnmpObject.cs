using Lextm.SharpSnmpLib;

namespace Engine.Pipeline
{
    /// <summary>
    /// SNMP object
    /// </summary>
    public interface ISnmpObject
    {
        /// <summary>
        /// Matches the GET NEXT criteria
        /// </summary>
        /// <param name="id">The ID in the GET NEXT message</param>
        /// <returns><c>null</c> if it does not match.</returns>
        ScalarObject? MatchGetNext(ObjectIdentifier id);

        /// <summary>
        /// Matches the GET criteria
        /// </summary>
        /// <param name="id">The ID in the GET message</param>
        /// <returns><c>null</c> if it does not match.</returns>
        ScalarObject? MatchGet(ObjectIdentifier id);
    }
}