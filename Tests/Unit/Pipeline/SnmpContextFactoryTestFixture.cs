using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using Moq;
using Engine.Pipeline;
using IListenerBinding = Engine.Pipeline.IListenerBinding;

namespace Tests.Unit.Pipeline
{
    public class SnmpContextFactoryTestFixture
    {
        [Fact]
        public void Test()
        {
            var engineId = ByteTool.Convert("80004fb805636c6f75644dab22cc".Cast<char>());

            var messageMock = new Mock<ISnmpMessage>();
            messageMock.Setup(foo => foo.Version).Returns(VersionCode.V3);
            var bindingMock = new Mock<IListenerBinding>();
            var context = SnmpContextFactory.Create(messageMock.Object, new IPEndPoint(IPAddress.Loopback, 0), new UserRegistry(),
                                      new EngineGroup(engineId),
                                      bindingMock.Object);
            context.SendResponse();
            bindingMock.Verify(foo => foo.SendResponse(It.IsAny<ISnmpMessage>(), It.IsAny<EndPoint>()), Times.AtMostOnce);
        }
    }
}