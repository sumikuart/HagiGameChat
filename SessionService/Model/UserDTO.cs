namespace SessionService.Model
{
    public class UserDTO
    {
        public string userName { get; set; }

        public string passwordHash { get; set; }

        public string guild { get; set; }

        public string role { get; set; }
    }
}
