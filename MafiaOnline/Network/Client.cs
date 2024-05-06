using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;

namespace MafiaOnline.Network
{
    internal class Client : User
    {
        private TcpClient? tcpClient;
        private int _serverPort = 9850;
        private IPAddress? _serverIP;

        public Client()
        {
            tcpClient = new TcpClient();
        }

        public async Task Join(Player player)
        {
            UdpClient udpClient = new(11000);
            udpClient.EnableBroadcast = true;

            IPEndPoint receiveEndPoint = new(IPAddress.Any, 0);
            UdpReceiveResult receiveResult = await udpClient.ReceiveAsync();
            byte[] receiveBytes = receiveResult.Buffer;
            string receivedData = Encoding.UTF8.GetString(receiveBytes);
            udpClient.Close();
            Console.WriteLine($"Received broadcast from {receiveResult.RemoteEndPoint} : {receivedData}\n");

            _serverIP = IPAddress.Parse(receivedData);
            tcpClient.Connect(_serverIP, _serverPort);

            NetworkStream stream = tcpClient.GetStream();
            {
                if (player != null)
                {
                    string json = JsonConvert.SerializeObject(player);
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                    await stream.WriteAsync(jsonBytes);
                }
            }
            Console.WriteLine($"Подключение к серверу: {tcpClient.Connected}");
            //while (true) { Console.WriteLine(tcpClient.Connected); }
        }

    }
}
