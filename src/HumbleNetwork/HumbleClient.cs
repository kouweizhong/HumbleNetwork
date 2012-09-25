namespace HumbleNetwork
{
    using System;
    using System.Net.Sockets;

    public class HumbleClient : IHumbleClient
    {
        private readonly Framing framing;
        private readonly string delimiter;
        private readonly TcpClient tcpClient = new TcpClient();
        private IHumbleStream stream;

        public HumbleClient(Framing framing = Framing.LengthPrefixed, string delimiter = MessageFraming.DefaultDelimiter)
        {
            this.framing = framing;
            this.delimiter = delimiter;
        }

        public int ReceiveTimeOut
        {
            get { return this.stream.NetworkStream.ReadTimeout; }
            set { this.stream.NetworkStream.ReadTimeout = value; }
        }

        public HumbleClient Send(string data)
        {
            this.stream.Send(data);
            return this;
        }

        public string Receive()
        {
            ////if (this.tcpClient.IsItReallyConnected() == false)
            ////{
            ////    throw new SocketException(10057);
            ////}

            var data = this.stream.Receive();
            this.stream.NetworkStream.Dispose();
            return data;
        }

        public HumbleClient Connect(string host, int port)
        {
            this.tcpClient.Connect(host, port);
            this.CreateStream();
            return this;
        }

        /// <summary>
        /// TODO: good practices on disconnect
        /// </summary>
        public void Dispose()
        {
            this.tcpClient.Close();
        }

        private void CreateStream()
        {
            this.stream = MessageFraming.Create(this.framing, this.tcpClient, this.delimiter);
        }
    }
}