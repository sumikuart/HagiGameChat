using GameService.Class;
using GameService.Models;
using System.Net;
using System.Net.Http.Headers;
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
            foreach (var ip in ipHostInfo.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static HttpClient SetupHttpClient(string address)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(address);
            client.Timeout = TimeSpan.FromSeconds(5);

            return client;
        }

        public static async Task<string> GetGuildAsync(string userName)
        {
            HttpClient httpLoginClient = SetupHttpClient("http://Login_api");
            HttpResponseMessage responseMessage = await httpLoginClient.GetAsync($"/api/User/GetUserData/{userName}");
            if (responseMessage.IsSuccessStatusCode)
            {
                string UserDTOString = await responseMessage.Content.ReadAsStringAsync();
                UserDTO FormatedResult = JsonSerializer.Deserialize<UserDTO>(UserDTOString);
                return FormatedResult.guild;
            }
            return "";
        }

        public static async Task Main(string[] args)
        {
            while (true)
            {
                TcpListener server = null;
                string CurrentUsername = "default";
                try
                {
                    // Set the TcpListener on port 13000.
                    Int32 port = Int32.Parse(Environment.GetEnvironmentVariable("PORT"));
                    server = new TcpListener(IPAddress.Any, port);
                    server.Start();
                    Console.WriteLine("Waiting for a connection... ");

                    Byte[] bytes = new Byte[512];
                    String data = null;

                    HttpClient httpClient = SetupHttpClient("http://GameserverRegister");

                    Console.WriteLine(GetIPAddress());
                    ServerIpDTO serverIpDTO = new ServerIpDTO() { ip = GetIPAddress(), port = port };
                    var JsonData = JsonSerializer.Serialize(serverIpDTO);
                    Console.WriteLine(JsonData);
                    StringContent content = new StringContent(JsonData, Encoding.UTF8, "application/json");
                    Console.WriteLine(content);
                    HttpResponseMessage response = await httpClient.PostAsync("api/GameServerReg/Register", content);
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    using TcpClient client = server.AcceptTcpClient();
                    Task myTask = Task.Run(async () =>
                    {
                        Console.WriteLine("the task has been started");
                        ServerIpDTO serverIpDTO = new ServerIpDTO() { ip = GetIPAddress(), port = 13000 };
                        var JsonData = JsonSerializer.Serialize(serverIpDTO);
                        StringContent content = new StringContent(JsonData, Encoding.UTF8, "application/json");
                        while (client.Connected)
                        {
                            HttpResponseMessage response = await httpClient.PostAsync("api/GameServerReg/Full", content);
                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine(await response.Content.ReadAsStringAsync());
                            }
                            Thread.Sleep(5000);
                        }
                        Console.WriteLine("i have stopped, my loop informing that im full");
                    });
                    Console.WriteLine("Connected!");
                    (int left, int top) currentPos = (0, 0);
                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();
                    int i;
                    HttpClient msgClient = SetupHttpClient("http://session_service_api");
                    data = null;
                    string guild = "";
                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        string message;
                        string return_message = "No such Command exsist or you missing a argument: " + data;
                        string[] line = data.Split(' ', 2);
                        if (line.Length == 2)
                        {
                            switch (line[0])
                            {
                                case "login":
                                    string[] loginString = line[1].Split(' ', 2);
                                    CurrentUsername = loginString[0];
                                    msgClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginString[1]);
                                    guild = await GetGuildAsync(CurrentUsername);
                                    Task.Run(() => MsgConsumer.RabbitConnection(guild, CurrentUsername, stream));
                                    return_message = $"you have succesfully logged on to the server";
                                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(return_message);
                                    stream.Write(msg, 0, msg.Length);
                                    break;

                                case "pos":
                                    string[] posString = line[1].Split(' ', 2);
                                    if (posString.Length == 2)
                                    {
                                        (int left, int top) pos = (Int32.Parse(posString[0]), Int32.Parse(posString[1]));
                                        if (pos.left >= 0 && pos.top >= 0 && pos.left <= 10 && pos.top <= 10)
                                        {
                                            currentPos = pos;
                                            return_message = $"the server has accepted you request and moved you to {currentPos.left}, {currentPos.top}";
                                        }
                                        return_message = $"the server has denied you request to move to {pos.left}, {pos.top}, you can only move within the range 0 to 10 /n your still at {currentPos.left}, {currentPos.top}";
         
                                    }
                                    byte[] msgPos = System.Text.Encoding.ASCII.GetBytes(return_message);
                                    stream.Write(msgPos, 0, msgPos.Length);
                                    break;

                                case "private":
                                    string[] UserMessage = line[1].Split(' ', 2);
                                    if (UserMessage.Length == 2)
                                    {
                                        string username = UserMessage[0];
                                        message = UserMessage[1];
                                        return_message = $"{CurrentUsername}->{username}: {message}";
                                        MessageForm messageToOutsidePrivate = new MessageForm()
                                        {
                                            User = CurrentUsername,
                                            Message = return_message,
                                            Target = username
                                        };
                                        var JsonDataPrivate = JsonSerializer.Serialize(messageToOutsidePrivate);
                                        StringContent contentPrivate = new StringContent(JsonDataPrivate, Encoding.UTF8, "application/json");
                                        HttpResponseMessage privateResponse = await msgClient.PostAsync("/api/MsgChat/SendPrivateMessage/PrivateMSG", contentPrivate);
                                        if (privateResponse.IsSuccessStatusCode || privateResponse.StatusCode == HttpStatusCode.NotFound)
                                        {
                                            string responseString = await privateResponse.Content.ReadAsStringAsync();
                                            if (!responseString.StartsWith("Private msg to :"))
                                            {
                                                return_message = responseString;
                                            }
                                        }
                                        byte[] msgPr = System.Text.Encoding.ASCII.GetBytes(return_message);
                                        stream.Write(msgPr, 0, msgPr.Length);
                                    }
                                    break;

                                case "guild":
                                    message = line[1];
                                    return_message = $"[{guild}]{CurrentUsername}: {message}";
                                    MessageForm messageToOutsideGuild = new MessageForm()
                                    {
                                        User = CurrentUsername,
                                        Message = return_message,
                                        Target = guild
                                    };
                                    var JsonDataGuild = JsonSerializer.Serialize(messageToOutsideGuild);
                                    StringContent contentGuild = new StringContent(JsonDataGuild, Encoding.UTF8, "application/json");
                                    HttpResponseMessage GuildResponse = await msgClient.PostAsync("/api/MsgChat/SendGuildMessage/GuildMSG", contentGuild);
                                    break;

                                case "global":
                                    message = line[1];
                                    return_message = $"Global message: {message}";
                                    MessageForm messageToOutsideGlobal = new MessageForm()
                                    {
                                        User = "",
                                        Message = return_message,
                                        Target = ""
                                    };
                                    var JsonDataGlobal = JsonSerializer.Serialize(messageToOutsideGlobal);
                                    StringContent contentGlobal = new StringContent(JsonDataGlobal, Encoding.UTF8, "application/json");
                                    HttpResponseMessage GlobalResponse = await msgClient.PostAsync("/api/MsgChat/SendPublicMessage/PublicMSG", contentGlobal);
 

                                    if (!GlobalResponse.IsSuccessStatusCode)
                                    {
                                        string responseStringGlobal = "Users dont have acces";
                                        byte[] msgGo = System.Text.Encoding.ASCII.GetBytes(responseStringGlobal);
                                        stream.Write(msgGo, 0, msgGo.Length);
                                    }  
                         
                                    break;
                            }
                        }
                        // Send back a response.
                        Console.WriteLine("Sent: {0}", data);
                    }
                    Console.WriteLine("stream stopped reading");
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }
                finally
                {
                    Console.WriteLine("i have stopped, and is gonna restart");
                    server.Stop();
                }
            }
        }
    }
}