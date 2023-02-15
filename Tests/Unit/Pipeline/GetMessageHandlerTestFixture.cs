using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using Moq;
using Engine.Objects;
using Engine.Pipeline;

namespace Tests.Unit.Pipeline
{
    public class GetMessageHandlerTestFixture
    {
        [Fact]
        public void NoSuchInstance()
        {
            var handler = new GetMessageHandler();
            var context = SnmpContextFactory.Create(
                new GetRequestMessage(
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
            handler.Handle(context, store);
            var nosuchinstance = (ResponseMessage)context.Response;
            Assert.Equal(new NoSuchInstance(), nosuchinstance.Variables()[0].Data);
        }

        [Fact]
        public void NoError()
        {
            var handler = new GetMessageHandler();
            var context = SnmpContextFactory.Create(
                new GetRequestMessage(
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
            Assert.Throws<ArgumentNullException>(() => handler.Handle(null, null));
            Assert.Throws<ArgumentNullException>(() => handler.Handle(context, null));
            handler.Handle(context, store);
            var noerror = (ResponseMessage)context.Response;
            Assert.Equal(ErrorCode.NoError, noerror.ErrorStatus);
        }

        [Fact]
        public void GenError()
        {
            var handler = new GetMessageHandler();
            var mock = new Mock<ScalarObject>(new ObjectIdentifier("1.3.6.1.2.1.1.2.0"));
            mock.Setup(foo => foo.Data).Throws<Exception>();
            mock.Setup(foo => foo.MatchGet(new ObjectIdentifier("1.3.6.1.2.1.1.2.0"))).Returns(mock.Object);
            var store = new ObjectStore();
            store.Add(mock.Object);
            var context = SnmpContextFactory.Create(
                new GetRequestMessage(
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
            var genError = (ResponseMessage)context.Response;
            Assert.Equal(ErrorCode.GenError, genError.ErrorStatus);
        }

        [Fact]
        public void NoSuchObject()
        {
            var handler = new GetMessageHandler();
            var mock = new Mock<ScalarObject>(new ObjectIdentifier("1.3.6.1.2.1.1.2.0"));
            mock.Setup(foo => foo.Data).Throws<AccessFailureException>();
            mock.Setup(foo => foo.MatchGet(new ObjectIdentifier("1.3.6.1.2.1.1.2.0"))).Returns(mock.Object);
            var store = new ObjectStore();
            store.Add(mock.Object);
            var context = SnmpContextFactory.Create(
                new GetRequestMessage(
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
            var noSuchObject = (ResponseMessage)context.Response;
            Assert.Equal(new NoSuchObject(), noSuchObject.Variables()[0].Data);
        }
    }
}