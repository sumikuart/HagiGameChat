using System.Net.Sockets;
using System.Net;

namespace SessionService.Class
{
    public class ConnectionHandler
    {

        public void ListenToPort(int listenPort)
        {
            TcpListener server = null;
            string CurrentUsername = "default";
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = listenPort;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                server.Start();

                Byte[] bytes = new Byte[256];
                String data = null;

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                using TcpClient client = server.AcceptTcpClient();

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

                        // Process the data sent by the client.
                        string message;
                        string return_message = "No such Command exsist or you missing a argument";
                        string[] line = data.Split('¤', 2);
                        if (line.Length == 2)
                        {
                            switch (line[0])
                            {
                                case "C_UO":                      
                                    return_message = $"you have succesfully logged on to the server";
                                    break;
                                
                            }
                        }
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(return_message);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);

                    }
                }
            }
            catch (SocketException e)
            {
               
            }
            finally
            {
                server.Stop();
            }


        }

    }
}
