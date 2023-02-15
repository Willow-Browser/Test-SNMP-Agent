using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    // TODO: this is not accessible. So how to handle?
    public sealed class SysORIndex : ScalarObject
    {
        private readonly ISnmpData data;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public SysORIndex(int index)
            : base("1.3.6.1.2.1.1.9.1.1.{0}", index)
        {
            data = new Integer32(index);
        }

        public override ISnmpData Data
        {
            get { return data; }
            set { throw new AccessFailureException(); }
        }
    }
}