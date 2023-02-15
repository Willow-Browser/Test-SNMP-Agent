using System.Net;
using Engine.Pipeline;

namespace Tests.Unit.Pipeline
{
    public class ListenerTestFixture
    {
        [Fact]
        public void AddBindingDuplicate()
        {
            Assert.Equal(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 21), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 21));
            var listener = new Listener();
            listener.AddBinding(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 21));
            listener.AddBinding(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 21));
            Assert.Equal(1, listener.Bindings.Count);
        }

        [Fact]
        public void RemoveBinding()
        {
            Assert.Equal(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 21), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 21));
            var listener = new Listener();
            listener.AddBinding(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 21));
            listener.RemoveBinding(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 21));
            Assert.Equal(0, listener.Bindings.Count);
        }
    }
}