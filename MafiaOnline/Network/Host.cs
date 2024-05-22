using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MafiaOnline.Network
{
    internal class Host : User
    {
        public bool ServerOn;
        public bool ClientsWaiting;

        private readonly TcpListener _listener;
        private JsonSerializerSettings _serializerSettings;

        public Dictionary<Player, TcpClient>? connections;

        public Host(int port)
        {
            ServerOn = false;
            IP = Tools.GetLocalAddress();
            Console.WriteLine(IP.ToString());
            Port = port;
            _listener = new TcpListener(this.IP, this.Port);
            connections = new Dictionary<Player, TcpClient>();
            _serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
            };
        }
        public void Start()
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

        //Отправка всем клиентам списка подключенных игроков
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
                                    string json = JsonConvert.SerializeObject(new { type = "playerList", players = currentPlayers }, _serializerSettings);
                                    byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                                    await stream.WriteAsync(jsonBytes);
                                }
                            }
                        }
                    }
                    await Task.Delay(2000);
                }
                catch (Exception ex) {Console.WriteLine("SendPlayers: " + ex.Message); }
            }
        }

        //Отправка пакеты, уведомляющиего о начале игры
        public async void SendStartPackage()
        {
            try
            {
                List<TcpClient> currentConnections = connections!.Values.ToList();
                int i = 0;
                foreach (TcpClient client in currentConnections)
                {
                    if (client.Connected)
                    {
                        NetworkStream stream = client.GetStream();
                        {
                            string json = JsonConvert.SerializeObject(new { type = "startGame", id = i++ }, _serializerSettings);
                            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                            await stream.WriteAsync(jsonBytes);
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("SendStartPackage: " + ex.Message); }
        }

        //Прослушивание клиентов
        public void ForwardUpdateGamePackages()
        {
            List<TcpClient> currentConnections = connections!.Values.ToList();
            foreach (var connection in currentConnections)
            {
                Thread clientThread = new Thread(() => HandleClient(connection));
                clientThread.Start();
            }
        }

        public void HandleClient(TcpClient client)
        {
            Game game = new();
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];
            while (ServerOn)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string? json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    try
                    {
                        JObject data = JObject.Parse(json);
                        Console.WriteLine(data.ToString());

                        if (data["type"]?.ToString() == "gameInfo")
                        {
                            JToken infoToken = data["info"];
                            if (infoToken != null)
                            {
                                game = infoToken.ToObject<Game>();
                                Console.WriteLine("Received updated game info!");
                                SendGameInfo(game);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка при десериализации JSON: " + ex.Message);
                        Console.WriteLine("JSON: " + json);
                    }
                }
            }
        }

        //Отправка экземпляра игры всем клиентам
        public async void SendGameInfo(Game game)
        {
            try
            {
                if (connections == null)
                {
                    Console.WriteLine("Error: Connections dictionary is null.");
                    return;
                }

                if (_serializerSettings == null)
                {
                    Console.WriteLine("Error: Serializer settings are null.");
                    return;
                }

                List<TcpClient> currentConnections = connections.Values.ToList();
                Console.WriteLine("Sending game info...");
                Console.WriteLine($"Serializer settings initialized: {_serializerSettings != null}");

                // Логирование JSON-структуры игры для отладки
                try
                {
                    string testJson = JsonConvert.SerializeObject(new { type = "gameInfo", info = game }, _serializerSettings);
                    Console.WriteLine("Game JSON preview:");
                    Console.WriteLine(testJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error serializing game object: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    return;
                }

                foreach (TcpClient client in currentConnections)
                {
                    if (client != null && client.Connected)
                    {
                        NetworkStream? stream = client.GetStream();
                        if (stream != null)
                        {
                            string json = JsonConvert.SerializeObject(new { type = "gameInfo", info = game }, _serializerSettings);
                            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                            await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
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

        private void AcceptClients()
        {
            int id = 0;
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

                        Player? player = JsonConvert.DeserializeObject<Player>(json, _serializerSettings);


                        if (!connections!.ContainsKey(player!))
                        {
                            player.id = id;
                            connections.Add(player!, tcpClient);
                            id++;
                        }

                        Console.WriteLine(connections.ToString());
                    }
                }
                catch (Exception ex) { Console.WriteLine("AcceptClients: " + ex.Message); }
            }
        }
    }
}