using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MafiaOnline.Network
{
    internal class Host : User, INotifyPropertyChanged
    {
        public bool ServerOn;
        public bool ClientsWaiting;

        private TcpListener _listener;

        public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<Player, TcpClient>? connections{ get; set; }

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
            UdpClient udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;
            IPEndPoint broadcastEndPoint = new IPEndPoint(Tools.GetBroadcastAddress(IP.ToString(), Tools.GetLocalMask().ToString()), 11000);
            try
            {
                ClientsWaiting = true;
                Thread accceptClientsThread = new(AcceptClients);
                accceptClientsThread.Start();
                while (ClientsWaiting)
                {
                    byte[] sendBytes = Encoding.UTF8.GetBytes(IP.ToString());
                    udpClient.Send(sendBytes, sendBytes.Length, broadcastEndPoint);
                    Console.WriteLine("Broadcast wac sended!");
                    await Task.Delay(1000);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                udpClient.Close();
            }

        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void AcceptClients()
        {
            Console.WriteLine("Сервер запущен...");
            while (ServerOn)
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

                    if (connections!.ContainsKey(player))
                    {
                        connections.Add(player, tcpClient);
                        
                    }
                    Console.WriteLine(connections.ToString());
                }
            }
        }
    }
}