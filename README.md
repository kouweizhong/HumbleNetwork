HumbleNetwork
=============

It's a library to help build network services based in sockets. It offers a client and a server class and a stream helper. See the example:

namespace HumbleExample
{
    using System;
    using HumbleNetwork;
    using HumbleNetwork.Streams;

    class Program
    {
        static void Main(string[] args)
        {
            var server = new HumbleServer();
            server.AddCommand("echo", () => new EchoCommand());
            server.Start(0);

            var client = new HumbleClient();
            client.Connect("localhost", server.Port);
            client.Send("echo").Send("hello world");

            Console.WriteLine("Client received: " + client.Receive());
            Console.ReadKey();
        }
    }

    public class EchoCommand : ICommand
    {
        public void Execute(IHumbleStream stream)
        {
            stream.Send(stream.Receive());
        }
    }
}

TODO
----

* Build script
* Accept send and receive stream
* Async calls for streams