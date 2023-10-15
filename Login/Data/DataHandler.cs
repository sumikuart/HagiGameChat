using Login.Model;

namespace Login.Data
{
    public static class DataHandler
    {

        public static List<UserDto> DtoUserList = new List<UserDto>();
        public static List<User> UserList = new List<User>();
        static string[] guilds = new string[2] { "Blue", "Yellow" };
        static string[] roles = new string[2] { "User", "Admin" };

        static DataHandler()
        {
            CreateData();
        }

        public static void CreateData()
        {
            UserDto UserA = new UserDto() {

                UserName = "Kim",
                Password = "Kim",
                Guild = guilds[0],
                Role = roles[1]
            };

            UserDto UserB = new UserDto()
            {

                UserName = "Mahtias",
                Password = "Mahtias",
                Guild = guilds[0],
                Role = roles[1]
            };

            UserDto UserC = new UserDto()
            {

                UserName = "Greg",
                Password = "Greg",
                Guild = guilds[1],
                Role = roles[0]
            };


            UserDto UserD = new UserDto()
            {

                UserName = "Alice",
                Password = "Alice",
                Guild = guilds[1],
                Role = roles[0]
            };

            UserDto UserE = new UserDto()
            {

                UserName = "Evan",
                Password = "Evan",
                Guild = guilds[0],
                Role = roles[0]
            };

            UserList.Add(ConvertUserDto(UserA));
            UserList.Add(ConvertUserDto(UserB));
            UserList.Add(ConvertUserDto(UserC));
            UserList.Add(ConvertUserDto(UserD));
            UserList.Add(ConvertUserDto(UserE));
        }


        private static User ConvertUserDto(UserDto DtoUser)
        {
            User Result = new User();

            Result.UserName = DtoUser.UserName;
            Result.PasswordHash = BCrypt.Net.BCrypt.HashPassword(DtoUser.UserName);
            Result.Guild = DtoUser.Guild;
            Result.Role = DtoUser.Role;

            return Result;

        }
    }
}
