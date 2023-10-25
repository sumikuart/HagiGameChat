using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    internal class Program
    {
        public const string acceptMessage = "the server has accepted you request and moved you to {currentPos.left}, {currentPos.top}";
        public const string seperatorString = "+----------------------------------------------------------------------------------------------------------------------------------+";

        public static HttpClient SetupHttpClient(string addresse)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(addresse);
            client.Timeout = TimeSpan.FromSeconds(5);

            return client;
        }

        private static async Task Main(string[] args)
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            LoginObject login = new LoginObject() { UserName = username, Password = password };
            //HttpClient LoginClient = SetupHttpClient("http://Login_api");
            HttpClient LoginClient = SetupHttpClient("http://localhost:8084");

            var JsonData = JsonSerializer.Serialize(login);
            StringContent login_content = new StringContent(JsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage login_message = await LoginClient.PostAsync("api/User/ValidateUser/Login", login_content);
            if (login_message.IsSuccessStatusCode)
            {
                string JWT = await login_message.Content.ReadAsStringAsync();
                // HttpClient client = SetupHttpClient("http://GameserverRegister");
                HttpClient client = SetupHttpClient("http://localhost:8080");
                //client.DefaultRequestHeaders.Add("Authorization", "bearer " + JWT);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWT);
                HttpResponseMessage message = await client.GetAsync("api/GameServerReg/GetServer");
                if (message.IsSuccessStatusCode)
                {
                    string content = await message.Content.ReadAsStringAsync();
                    ServerIpDTO serverIp = JsonSerializer.Deserialize<ServerIpDTO>(content);
                    if (serverIp.ip != "")
                    {
                        //startClient(username, serverIp.ip, serverIp.port);
                        startClient(username, "localhost", serverIp.port);
                    }
                    else
                    {
                        Console.WriteLine("No open Servers");
                    }
                }
                else { Console.WriteLine("something wrong"); }
            }
        }

        private static void startClient(string username, string ip, int port)
        {
            try
            {
                Console.WriteLine($"start client on {ip}:{port}");
                Console.Clear();
                Thread.Sleep(500);
                using TcpClient client = new TcpClient(ip, port);
                NetworkStream stream = client.GetStream();

                //send username to gameserver
                Byte[] login_data = System.Text.Encoding.ASCII.GetBytes("login " + username);
                stream.Write(login_data, 0, login_data.Length);
                Task.Run(() =>
                {
                    (int left, int top) pos = (0, 0);
                    List<string> messages = new List<string>();
                    while (client.Connected)
                    {
                        // Receive the server response.
                        Byte[] received_data = new Byte[256];
                        String responseData = String.Empty;
                        Int32 bytes = stream.Read(received_data, 0, received_data.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(received_data, 0, bytes);
                        if (responseData.StartsWith(acceptMessage))
                        {
                            responseData.Remove(0, acceptMessage.Length);
                            string[] postions = responseData.Split(',', 2);
                            pos = (Int32.Parse(postions[0]), Int32.Parse(postions[1]));
                        }
                        else
                        {
                            if (seperatorString.Length - responseData.Length > 0)
                            {
                                responseData += new string(' ', (seperatorString.Length - responseData.Length));
                            }
                            messages.Add(responseData);
                        }
                        int cursorPos = Console.GetCursorPosition().Left;
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine(seperatorString);
                        Console.WriteLine($"Current Postion: {pos.left}, {pos.top}                            ");
                        Console.WriteLine(seperatorString);
                        foreach (string message in messages.TakeLast(10))
                        {
                            Console.WriteLine(message);
                        }
                        Console.WriteLine(seperatorString);
                        Console.SetCursorPosition(cursorPos, Console.GetCursorPosition().Top);
                    }
                });
                while (client.Connected)
                {
                    string message = Console.ReadLine();

                    if (message != null)
                    {
                        Byte[] send_data = System.Text.Encoding.ASCII.GetBytes(message);
                        stream.Write(send_data, 0, send_data.Length);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}