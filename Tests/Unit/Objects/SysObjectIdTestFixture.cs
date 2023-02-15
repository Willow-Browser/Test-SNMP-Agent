using Lextm.SharpSnmpLib;
using Engine.Objects;
using Engine.Pipeline;

namespace Tests.Unit.Objects
{
    public class SysObjectIdTestFixture
    {
        [Fact]
        public void Test()
        {
            var sys = new SysObjectId();
            Assert.Throws<AccessFailureException>(() => sys.Data = OctetString.Empty);
        }
    }
}