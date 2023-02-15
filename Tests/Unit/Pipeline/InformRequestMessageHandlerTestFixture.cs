using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Moq;
using Engine.Pipeline;
using IListenerBinding = Engine.Pipeline.IListenerBinding;

namespace Tests.Unit.Pipeline
{
    public class InformRequestMessageHandlerTestFixture
    {
        [Fact]
        public void Test()
        {
            var mock = new Mock<ISnmpContext>();
            var mock2 = new Mock<IListenerBinding>();
            IList<Variable> v = new List<Variable>();
            var message = new InformRequestMessage(0, VersionCode.V1, new OctetString("community"), new ObjectIdentifier("1.3.6"), 0, v);
            mock.Setup(foo => foo.Binding).Returns(mock2.Object);
            mock.Setup(foo => foo.Request).Returns(message);
            mock.Setup(foo => foo.Sender).Returns(new IPEndPoint(IPAddress.Any, 0));
            mock.Setup(foo => foo.CopyRequest(ErrorCode.NoError, 0)).Verifiable("this must be called");
            var handler = new InformRequestMessageHandler();
            Assert.Throws<ArgumentNullException>(() => handler.Handle(null, null));
            Assert.Throws<ArgumentNullException>(() => handler.Handle(mock.Object, null));
            handler.MessageReceived += delegate (object args, InformRequestMessageReceivedEventArgs e)
                                           {
                                               Assert.Equal(mock2.Object, e.Binding);
                                               Assert.Equal(message, e.InformRequestMessage);
                                               Assert.True(new IPEndPoint(IPAddress.Any, 0).Equals(e.Sender));
                                           };
            handler.Handle(mock.Object, new ObjectStore());
        }
    }
}