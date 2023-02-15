using Lextm.SharpSnmpLib;
using Engine.Pipeline;
using Engine.Objects;

namespace Tests.Unit.Objects
{
    public class SysORUpTimeTestFixture
    {
        [Fact]
        public void Test()
        {
            var sys = new SysORUpTime(3, new TimeTicks(3));
            Assert.Equal("1.3.6.1.2.1.1.9.1.4.3", sys.Variable.Id.ToString());
            Assert.Equal("00:00:00.0300000", sys.Data.ToString());
            Assert.Throws<AccessFailureException>(() => sys.Data = OctetString.Empty);
        }
    }
}