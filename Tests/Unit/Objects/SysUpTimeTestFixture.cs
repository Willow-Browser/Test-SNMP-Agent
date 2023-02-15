using Lextm.SharpSnmpLib;
using Engine.Objects;
using Engine.Pipeline;

namespace Tests.Unit.Objects
{
    public class SysUpTimeTestFixture
    {
        [Fact]
        public void Test()
        {
            var sys = new SysUpTime();
            Assert.Throws<AccessFailureException>(() => sys.Data = OctetString.Empty);
        }
    }
}