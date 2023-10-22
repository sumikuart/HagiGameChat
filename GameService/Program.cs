using GameService.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace GameService
{
    internal class Program
    {
        public static string GetIPAddress()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress iPAddress = ipHostInfo.AddressList[0];

            return iPAddress.ToString();
        }

        public static HttpClient SetupHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://gameserver_register/api/GameServerReg");
            client.Timeout = TimeSpan.FromSeconds(5);

            return client;
        }
        public static async void Main(string[] args)
        {
            TcpListener server = null;
            string CurrentUsername = "default";
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                server.Start();
                Console.WriteLine("Waiting for a connection... ");

                Byte[] bytes = new Byte[256];
                String data = null;

                HttpClient httpClient = SetupHttpClient();

                ServerInfoDTO serverInfoDTO = new ServerInfoDTO() { ip = GetIPAddress(), port = port };
                var JsonData = JsonSerializer.Serialize(serverInfoDTO);
                StringContent content = new StringContent(JsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("Register", content);
                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                using TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();
                int i;
                while (client.Connected)
                {
                    data = null;
                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        string message;
                        string return_message = "No such Command exsist or you missing a argument";
                        string[] line = data.Split(' ', 2);
                        if(line.Length == 2)
                        {
                            switch (line[0])
                            {
                                case "login":
                                    CurrentUsername = line[1];
                                    return_message = $"you have succesfully logged on to the server";
                                    break;
                                case "pos":
                                    string[] posString = line[1].Split(' ', 2);
                                    if(posString.Length == 2)
                                    {
                                        (int left, int top) pos = (Int32.Parse(posString[0]), Int32.Parse(posString[1]));
                                        return_message = $"the server has accepted you request and moved you to {pos.left}, {pos.top}";
                                    }
                                    break;
                                case "private":
                                    string[] UserMessage = line[1].Split(' ', 2);
                                    if(UserMessage.Length == 2)
                                    {
                                        string username = UserMessage[0];
                                        message = UserMessage[1];
                                        return_message = $"{CurrentUsername}->{username}: {message}";
                                    }
                                    break;
                                case "guild":
                                    string[] GuildMessage = line[1].Split(' ', 2);
                                    if(GuildMessage.Length == 2)
                                    {
                                        string guildName = GuildMessage[0];
                                        message = GuildMessage[1];
                                        return_message = $"[{guildName}]{CurrentUsername}: {message}";
                                    }
                                    break;
                                case "global":
                                    message = line[1];
                                    return_message = $"Global message: {message}";
                                    break;
                            }
                        }
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(return_message);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}