using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Tmds.MDns;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;

namespace MafiaOnline.Network
{
    internal class Client : User
    {
        private TcpClient? tcpClient;
        private int _serverPort = 9850;
        private IPAddress _serverIP;

        public Client()
        {
            tcpClient = new TcpClient();
        }

        public void Join()
        {
            UdpClient udpClient = new UdpClient(11000);
            udpClient.EnableBroadcast = true;

            IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receiveBytes = udpClient.Receive(ref receiveEndPoint);
            string receivedData = Encoding.ASCII.GetString(receiveBytes);
            udpClient.Close();
            Console.WriteLine($"Received broadcast from {receiveEndPoint.ToString()} : {receivedData}\n");

            _serverIP = IPAddress.Parse(receivedData);
            tcpClient.Connect(_serverIP, _serverPort);
            Console.WriteLine($"Подключение к серверу: {tcpClient.Connected}");
        }
    }
}
