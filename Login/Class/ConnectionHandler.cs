using Login.Model;
using System.IO;
using System.Net.Sockets;

namespace Login.Class
{
    public class ConnectionHandler
    {

        NetworkStream _stream;
        static string NewestResponse; 

        public string SendMsg(string Msg) {
            //send username to gameserver
            Byte[] login_data = System.Text.Encoding.ASCII.GetBytes(Msg);
            _stream.Write(login_data, 0, login_data.Length);

            while(NewestResponse == null)
            {

            }

            return NewestResponse;

        }
        public void CreateConnection(string ip, int port)
        {

            

            try
            {
                
                using TcpClient client = new TcpClient(ip, port);
                NetworkStream stream = client.GetStream();
                _stream = stream;

                Task.Run(() =>
                {
                    while (client.Connected)
                    {
                        // Receive the server response.
                        Byte[] received_data = new Byte[256];
                        String responseData = String.Empty;
                        Int32 bytes = stream.Read(received_data, 0, received_data.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(received_data, 0, bytes);
                        NewestResponse = responseData;
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
