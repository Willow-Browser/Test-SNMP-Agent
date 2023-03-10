using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using Moq;
using Engine.Objects;
using Engine.Pipeline;

namespace Tests.Unit.Pipeline
{
    public class GetNextMessageHandlerTestFixture
    {
        [Fact]
        public void NoError()
        {
            var handler = new GetNextMessageHandler();
            var context = SnmpContextFactory.Create(
                new GetNextRequestMessage(
                    300,
                    VersionCode.V1,
                    new OctetString("lextm"),
                    new List<Variable>
                        {
                            new Variable(new ObjectIdentifier("1.3.6.1.2.1.1.1.0"))
                        }
                    ),
                new IPEndPoint(IPAddress.Loopback, 100),
                new UserRegistry(),
                null,
                null);
            var store = new ObjectStore();
            store.Add(new SysDescr());
            store.Add(new SysObjectId());
            Assert.Throws<ArgumentNullException>(() => handler.Handle(null, null));
            Assert.Throws<ArgumentNullException>(() => handler.Handle(context, null));
            handler.Handle(context, store);
            var noerror = (ResponseMessage)context.Response;
            Assert.Equal(ErrorCode.NoError, noerror.ErrorStatus);
            Assert.Equal(new ObjectIdentifier("1.3.6.1.2.1.1.2.0"), noerror.Variables()[0].Id);
        }

        [Fact]
        public void GenError()
        {
            var handler = new GetNextMessageHandler();
            var mock = new Mock<ScalarObject>(new ObjectIdentifier("1.3.6.1.2.1.1.2.0"));
            mock.Setup(foo => foo.Data).Throws<Exception>();
            mock.Setup(foo => foo.MatchGet(new ObjectIdentifier("1.3.6.1.2.1.1.2.0"))).Returns(mock.Object);
            mock.Setup(foo => foo.MatchGetNext(new ObjectIdentifier("1.3.6.1.2.1.1.1.0"))).Returns(mock.Object);
            var store = new ObjectStore();
            store.Add(new SysDescr());
            store.Add(mock.Object);
            var context = SnmpContextFactory.Create(
                new GetNextRequestMessage(
                    300,
                    VersionCode.V1,
                    new OctetString("lextm"),
                    new List<Variable>
                        {
                            new Variable(new ObjectIdentifier("1.3.6.1.2.1.1.1.0"))
                        }
                    ),
                new IPEndPoint(IPAddress.Loopback, 100),
                new UserRegistry(),
                null,
                null);
            handler.Handle(context, store);
            var genError = (ResponseMessage)context.Response;
            Assert.Equal(ErrorCode.GenError, genError.ErrorStatus);
        }

        [Fact]
        public void EndOfMibView()
        {
            var handler = new GetNextMessageHandler();
            var store = new ObjectStore();
            store.Add(new SysDescr());
            var context = SnmpContextFactory.Create(
                new GetNextRequestMessage(
                    300,
                    VersionCode.V1,
                    new OctetString("lextm"),
                    new List<Variable>
                        {
                            new Variable(new ObjectIdentifier("1.3.6.1.2.1.1.2.0"))
                        }
                    ),
                new IPEndPoint(IPAddress.Loopback, 100),
                new UserRegistry(),
                null,
                null);
            handler.Handle(context, store);
            var endOfMibView = (ResponseMessage)context.Response;
            Assert.Equal(new ObjectIdentifier("1.3.6.1.2.1.1.2.0"), endOfMibView.Variables()[0].Id);
            Assert.Equal(new EndOfMibView(), endOfMibView.Variables()[0].Data);
        }
    }
}