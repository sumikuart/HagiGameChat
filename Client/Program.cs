using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Client
{
    internal class Program
    {
        public const string acceptMessage = "the server has accepted you request and moved you to {currentPos.left}, {currentPos.top}";
        public const string seperatorString = "+----------------------------------------------------------------------------------------------------------------------------------+";
        public static HttpClient SetupHttpClient()
        {
            HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://GameserverRegister");
            client.BaseAddress = new Uri("http://localhost:8080");
            client.Timeout = TimeSpan.FromSeconds(5);

            return client;
        }

        static async Task Main(string[] args)
        {
          
    
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            //username = "Mathias";
            HttpClient client = SetupHttpClient();
            HttpResponseMessage message = await client.GetAsync("api/GameServerReg/GetServer");
            if(message.IsSuccessStatusCode)
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
            } else { Console.WriteLine("something wrong"); }
        }


       

        static void startClient(string username, string ip, int port)
        {
            try
            {
                Console.WriteLine($"start client on {ip}:{port}");
                using TcpClient client = new TcpClient(ip, port);
                NetworkStream stream = client.GetStream();

                //send username to gameserver
                Byte[] login_data = System.Text.Encoding.ASCII.GetBytes("login "+username);
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
                            if(seperatorString.Length-responseData.Length > 0)
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
                        foreach(string message in messages.TakeLast(10))
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

                    if(message != null)
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