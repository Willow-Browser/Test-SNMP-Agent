using Lextm.SharpSnmpLib;
using Engine.Pipeline;

namespace Engine.Objects
{
    public sealed class SysORID : ScalarObject
    {
        private readonly ObjectIdentifier data;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public SysORID(int index, ObjectIdentifier dots)
            : base("1.3.6.1.2.1.1.9.1.2.{0}", index)
        {
            data = dots;
        }

        public override ISnmpData Data
        {
            get { return data; }
            set { throw new AccessFailureException(); }
        }
    }
}