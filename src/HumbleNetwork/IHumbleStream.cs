namespace HumbleNetwork
{
    using System.Net.Sockets;

    public interface IHumbleStream
    {
        void Send(string message);

        string Receive();

        string Receive(int length);

        /// <summary>
        /// TODO: encapsulate
        /// </summary>
        NetworkStream NetworkStream
        {
            get;
        }

        TcpClient TcpClient
        {
            get;
        }
    }
}