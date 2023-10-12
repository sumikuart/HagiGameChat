using Login.Model;

namespace Login.Data
{
    public static class DataHandler
    {

        public static List<User> UserList = new List<User>();
        static string[] guilds = new string[2] { "Blue", "Yellow" };
        static string[] roles = new string[2] { "User", "Admin" };

        static DataHandler()
        {
            CreateData();
        }

        public static void CreateData()
        {
            User UserA = new User() {

                UserName = "Kim",
                Password = "Kim",
                Guild = guilds[0],
                Rank = roles[1]
            };

            User UserB = new User()
            {

                UserName = "Mahtias",
                Password = "Mahtias",
                Guild = guilds[0],
                Rank = roles[1]
            };

            User UserC = new User()
            {

                UserName = "Greg",
                Password = "Greg",
                Guild = guilds[1],
                Rank = roles[0]
            };


            User UserD = new User()
            {

                UserName = "Alice",
                Password = "Alice",
                Guild = guilds[1],
                Rank = roles[0]
            };

            User UserE = new User()
            {

                UserName = "Evan",
                Password = "Evan",
                Guild = guilds[0],
                Rank = roles[0]
            };

            UserList.Add(UserA);
            UserList.Add(UserB);
            UserList.Add(UserC);
            UserList.Add(UserD);
            UserList.Add(UserE);
        }

    }
}
