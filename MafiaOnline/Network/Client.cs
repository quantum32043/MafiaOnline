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
        public bool waitingServer;

        public Client()
        {
            //tcpClient = new TcpClient();
        }

        public async Task Join(Player player)
        {
            tcpClient = new TcpClient();
            UdpClient udpClient = new(11000);
            udpClient.EnableBroadcast = true;

            IPEndPoint receiveEndPoint = new(IPAddress.Any, 0);
            UdpReceiveResult receiveResult = await udpClient.ReceiveAsync();
            byte[] receiveBytes = receiveResult.Buffer;
            string receivedData = Encoding.UTF8.GetString(receiveBytes);
            udpClient.Close();
            Console.WriteLine($"Received broadcast from {receiveResult.RemoteEndPoint} : {receivedData}\n");

            _serverIP = IPAddress.Parse(receivedData);
            try
            {
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
                waitingServer = true;
                Console.WriteLine($"Подключение к серверу: {tcpClient.Connected}");
            }
            catch (Exception ex) { Console.WriteLine("1: " + ex.Message); }
        }

        public void Disconnect()
        {
            tcpClient.Close();
        }

        public List<Player> ReceivePreStartInfo()
        {
            List<Player>? players = new();
            if (tcpClient.Connected)
            {
                try
                {
                    NetworkStream stream = tcpClient.GetStream();
                    byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string? json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        try
                        {
                            dynamic data = JsonConvert.DeserializeObject(json);
                            Console.WriteLine(data.ToString());
                            if (data.type == "playerList")
                            {
                                players = JsonConvert.DeserializeObject<List<Player>>(data.players.ToString());
                            }
                            else if (data.type == "startGame")
                            {
                                waitingServer = false;
                                Console.WriteLine("Geted startGame package!");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Ошибка при десериализации JSON: " + ex.Message);
                            Console.WriteLine("JSON: " + json);
                        }
                    }
                    Console.WriteLine(players);
                }
                catch (Exception ex) { Console.WriteLine("2: " + ex.Message); }
            }
            else { Console.WriteLine("Disconnected!"); }
            return players;
        }
    }

}

