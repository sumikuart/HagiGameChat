﻿namespace Login.Model
{
 
    public class User
    {


        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string Guild { get; set; }

        public string Role { get; set; }
    }
}
