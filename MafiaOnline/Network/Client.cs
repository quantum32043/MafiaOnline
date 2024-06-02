using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MafiaOnline.Network
{
    internal class Client : User
    {
        private TcpClient? tcpClient;
        private int _serverPort = 9850;
        private IPAddress? _serverIP;
        public bool waitingServer;
        public int id;
        private JsonSerializerSettings _serializerSettings;
        private JsonSerializer _serializer;
        public event Action<Game> GameReceived;
        public event EventHandler? ActionCompleted;

        public Client()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            _serializer = JsonSerializer.Create(_serializerSettings);
        }

        public class ObjectWrapper()
        {
            public string Type { get; set; }
            public Game Info { get; set; }
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
                            JObject data = JObject.Parse(json);

                            Console.WriteLine(data.ToString());

                            if (data["type"]?.ToString() == "playerList")
                            {
                                // Извлекаем поле "players" и десериализуем его отдельно
                                JToken playersToken = data["players"];
                                if (playersToken != null)
                                {
                                    players = playersToken.ToObject<List<Player>>();
                                }
                            }
                            else if (data["type"]?.ToString() == "startGame")
                            {
                                waitingServer = false;
                                id = data["id"]?.ToObject<int>() ?? 0;
                                Console.WriteLine($"Received startGame package with Player id = {id}!");
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

        //Получение обновленных игровых данных
        public async Task ReceiveGameInfo()
        {
            while (true)
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
                            var deserializedGameWrapper = JsonConvert.DeserializeObject<ObjectWrapper>(json, _serializerSettings);
                            OnGameReceived(deserializedGameWrapper.Info); // передаем новую игру
                            OnActionCompleted();
                            Console.WriteLine(deserializedGameWrapper.Info.GetType());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Ошибка при десериализации JSON: " + ex.Message);
                            Console.WriteLine("JSON: " + json);
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("2: " + ex.Message); }
            }
        }

        private void OnGameReceived(Game game)
        {
            GameReceived?.Invoke(game);
        }

        protected virtual void OnActionCompleted()
        {
            ActionCompleted?.Invoke(this, EventArgs.Empty);
        }

        //Отправка клиентом своего обновленного экземпляра игры на хост
        public async void SendUpdatedGameInfo(Game game)
        {
            NetworkStream stream = tcpClient.GetStream();
            {
                string json = JsonConvert.SerializeObject(new { type = "gameInfo", info = game }, _serializerSettings);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                await stream.WriteAsync(jsonBytes);
            }
        }
    }

}

