using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;

namespace MafiaOnline.Network
{
    internal class Host : User//, INotifyPropertyChanged
    {
        public bool ServerOn;
        public bool ClientsWaiting;

        private TcpListener _listener;

        //public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<Player, TcpClient>? connections;

        public Host(int port)
        {
            ServerOn = false;
            IP = Tools.GetLocalAddress();
            Console.WriteLine(IP.ToString());
            Port = port;
            _listener = new TcpListener(this.IP, this.Port);
            connections = new Dictionary<Player, TcpClient>();
        }
        public async void Start()
        {
            _listener.Start();
            ServerOn = true;
            Thread udpBroadcast = new Thread(Broadcasting);
            udpBroadcast.Start();
            ClientsWaiting = true;
            Thread accceptClientsThread = new(AcceptClients);
            accceptClientsThread.Start();
            Thread playerDistributionThread = new(SendPlayers);
            playerDistributionThread.Start();
        }

        //public void OnPropertyChanged([CallerMemberName] string prop = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        //}

        public async void SendPlayers()
        {
            //CheckConnections();
            List<TcpClient> currentConnections = connections!.Values.ToList();
            List<Player> currentPlayers = connections.Keys.ToList();

            while (ClientsWaiting)
            {
                try
                {
                    currentConnections = connections!.Values.ToList();
                    currentPlayers = connections.Keys.ToList();
                    if (currentPlayers.Count > 0)
                    {
                        foreach (TcpClient client in currentConnections)
                        {
                            if (client.Connected)
                            {
                                NetworkStream stream = client.GetStream();
                                {
                                    string json = JsonConvert.SerializeObject(new { type = "playerList", players = currentPlayers });
                                    byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                                    await stream.WriteAsync(jsonBytes);
                                }
                            }
                        }
                    }
                    await Task.Delay(2000);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }

        public async void SendStartPackage()
        {
            try
            {
                List<TcpClient> currentConnections = connections!.Values.ToList();
                foreach (TcpClient client in currentConnections)
                {
                    if (client.Connected)
                    {
                        NetworkStream stream = client.GetStream();
                        {
                            string json = JsonConvert.SerializeObject(new { type = "startGame" });
                            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                            await stream.WriteAsync(jsonBytes);
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        //private void CheckConnections()
        //{
        //}

        private async void Broadcasting()
        {
            UdpClient udpClient = new();
            udpClient.EnableBroadcast = true;
            IPEndPoint broadcastEndPoint = new(Tools.GetBroadcastAddress(IP.ToString(), Tools.GetLocalMask().ToString()), 11000);
            try
            {
                ClientsWaiting = true;
                while (ClientsWaiting)
                {
                    byte[] sendBytes = Encoding.UTF8.GetBytes(IP.ToString());
                    udpClient.Send(sendBytes, sendBytes.Length, broadcastEndPoint);
                    Console.WriteLine("Broadcast wac sended!");
                    await Task.Delay(1000);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                udpClient.Close();
            }
        }

        private async void AcceptClients()
        {
            Console.WriteLine("Сервер запущен...");
            while (ClientsWaiting)
            {
                try
                {
                    var tcpClient = _listener.AcceptTcpClient();
                    Console.WriteLine($"{tcpClient.Client.RemoteEndPoint}");
                    NetworkStream stream = tcpClient.GetStream();
                    byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string? json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        Player? player = JsonConvert.DeserializeObject<Player>(json);


                        if (!connections!.ContainsKey(player))
                        {
                            connections.Add(player, tcpClient);

                        }

                        Console.WriteLine(connections.ToString());
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }
    }
}