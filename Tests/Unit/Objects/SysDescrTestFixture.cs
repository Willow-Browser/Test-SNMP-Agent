using Lextm.SharpSnmpLib;
using Engine.Objects;
using Engine.Pipeline;

namespace Tests.Unit.Objects
{
    public class SysDescrTestFixture
    {
        [Fact]
        public void Test()
        {
            var sys = new SysDescr();
            Assert.Throws<AccessFailureException>(() => sys.Data = OctetString.Empty);
        }
    }
}