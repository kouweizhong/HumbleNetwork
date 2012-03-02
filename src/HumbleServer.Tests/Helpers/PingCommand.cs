namespace HumbleServer.Tests.Helpers
{
    using System.Net.Sockets;
    using HumbleServer.Streams;

    public class PingCommand : ICommand
    {
        public void Execute(TcpClient client, IHumbleStream stream)
        {
            stream.Send("PONG");
        }
    }
}