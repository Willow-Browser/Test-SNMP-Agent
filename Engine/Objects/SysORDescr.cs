using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    public sealed class SysORDescr : ScalarObject
    {
        private readonly OctetString data;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public SysORDescr(int index, OctetString description)
            : base("1.3.6.1.2.1.1.9.1.3.{0}", index)
        {
            data = description;
        }

        public override ISnmpData Data
        {
            get { return data; }
            set { throw new AccessFailureException(); }
        }
    }
}