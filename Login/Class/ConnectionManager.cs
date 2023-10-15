namespace Login.Class
{
    public static class ConnectionManager
    {
        public static ConnectionHandler SessionServiceConnection = new ConnectionHandler();
        public static void CreateConnections()
        {
            SessionServiceConnection.CreateConnection("LocalHost", 16400);
        }
    }
}
