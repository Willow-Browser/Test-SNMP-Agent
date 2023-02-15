using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using Engine.Pipeline;
using System.Net;
using Listener = Engine.Pipeline.Listener;
using MessageReceivedEventArgs = Engine.Pipeline.MessageReceivedEventArgs;

// See https://aka.ms/new-console-template for more information
namespace snmpd
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length != 0)
            {
                return;
            }

            // var idEngine161 = ByteTool.Convert("80004fb805636c6f75644dab22cc");

            Console.WriteLine("Hello, World!");
        }

        private static void Engine_ExceptionRaised(object? sender, ExceptionRaisedEventArgs e)
        {
            Console.WriteLine("Exception occurred: {0}", e.Exception);
        }

        private static void RequestReceived(object? sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("Message version {0}: {1}", e.Message.Version, e.Message);
        }
    }
}

