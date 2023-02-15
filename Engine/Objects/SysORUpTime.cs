using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    public sealed class SysORUpTime : ScalarObject
    {
        private readonly TimeTicks data;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public SysORUpTime(int index, TimeTicks time)
            : base("1.3.6.1.2.1.1.9.1.4.{0}", index)
        {
            data = time;
        }

        public override ISnmpData Data
        {
            get { return data; }
            set { throw new AccessFailureException(); }
        }
    }
}