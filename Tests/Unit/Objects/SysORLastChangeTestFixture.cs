using Lextm.SharpSnmpLib;
using Engine.Pipeline;
using Engine.Objects;

namespace Tests.Unit.Objects
{
    public class SysORLastChangeTestFixture
    {
        [Fact]
        public void Test()
        {
            var sys = new SysORLastChange();
            Assert.Equal(new TimeTicks(0), sys.Data);
            Assert.Throws<AccessFailureException>(() => sys.Data = OctetString.Empty);
        }
    }
}