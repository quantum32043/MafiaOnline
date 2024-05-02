using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace MafiaOnline.Network
{
    internal class Host : User
    {
        public bool ServerOn;
        public bool ClientsWaiting;

        private TcpListener _listener;

        private Dictionary<Player, TcpClient>? connections;

        public Host(int port)
        {
            ServerOn = false;
            IP = Tools.GetLocalAddress();
            Console.WriteLine(IP.ToString());
            Port = port;
            _listener = new TcpListener(this.IP, this.Port);
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
                    // Преобразуем IP-адрес в байты
                    byte[] sendBytes = Encoding.ASCII.GetBytes(IP.ToString());
                    // Отправляем UDP-пакет
                    udpClient.Send(sendBytes, sendBytes.Length, broadcastEndPoint);
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

        private void AcceptClients()
        {
            Console.WriteLine("Сервер запущен...");
            while (ServerOn)
            {
                var tcpClient = _listener.AcceptTcpClient();
                Console.WriteLine($"{tcpClient.Client.RemoteEndPoint}");
            }
        }
    }
}
