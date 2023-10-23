using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Client
{
    internal class Program
    {
        public static HttpClient SetupHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://GameserverRegister");
            client.Timeout = TimeSpan.FromSeconds(5);

            return client;
        }

        static async Task Main(string[] args)
        {
          
    
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            HttpClient client = SetupHttpClient();
            HttpResponseMessage message = await client.GetAsync("api/GameServerReg/GetServer");
            if(message.IsSuccessStatusCode)
            {
                string content = await message.Content.ReadAsStringAsync();
                ServerIpDTO serverIp = JsonSerializer.Deserialize<ServerIpDTO>(content);
                if(serverIp.ip != "")
                {
                    startClient(username, serverIp.ip, serverIp.port);
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
                using TcpClient client = new TcpClient(ip, port);
                NetworkStream stream = client.GetStream();

                //send username to gameserver
                Byte[] login_data = System.Text.Encoding.ASCII.GetBytes("login "+username);
                stream.Write(login_data, 0, login_data.Length);

                Task.Run(() =>
                {
                    while (client.Connected)
                    {
                        // Receive the server response.
                        Byte[] received_data = new Byte[256];
                        String responseData = String.Empty;
                        Int32 bytes = stream.Read(received_data, 0, received_data.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(received_data, 0, bytes);
                        Console.WriteLine("Received: {0}", responseData);
                    }
                });
                while (client.Connected)
                {
                    string message = Console.ReadLine();

                    Byte[] send_data = System.Text.Encoding.ASCII.GetBytes(message);
                    stream.Write(send_data, 0, send_data.Length);

                    Console.WriteLine("Sent: {0}", message);
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