using Lextm.SharpSnmpLib;
using Engine.Pipeline;
using Engine.Objects;

namespace Tests.Unit.Objects
{
    public class SysServicesTestFixture
    {
        [Fact]
        public void Test()
        {
            var sys = new SysServices();
            Assert.Equal(new Integer32(72), sys.Data);
            Assert.Throws<AccessFailureException>(() => sys.Data = new TimeTicks(0));
        }
    }
}