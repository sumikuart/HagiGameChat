using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using SessionService.Data;
using SessionService.Model;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.Http.Json;


namespace SessionService.Class
{
    public static class LogicFunctions
    {


        public static SessionUserDataDTO GatherUserData(string userData)
        {
               
           Task<SessionUserDataDTO> task = null;
          
            task = GatherUserDataFromLogin(userData);
            task.Wait();
 

            return task.Result;
        }

        public static bool UserOnlineData(string userData)
        {

            Task<UserDTO> task = null;

            task = CheckUserInLogin(userData);
            task.Wait();

            bool userExcist = false;

            if (task.Result != null)
            {
                userExcist = true;
            } else
            {
                userExcist = false;
            }

      

            return userExcist;
        }

        public async static Task<UserDTO> CheckUserInLogin(string userName)
        {

            SessionUserDataDTO newData = new SessionUserDataDTO();
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true
            };



            string baseUrl = "http://login_api:80/api/";
            string PropUrl = "GetUserData/" + userName;
            string extendenUrl = baseUrl + "User/" + PropUrl;
            string result = "Req URL: " + extendenUrl + " - ";

            HttpClient client = new HttpClient(handler);
            HttpResponseMessage response = await client.GetAsync(extendenUrl);

            UserDTO FormatedResult = null;

            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
                FormatedResult = JsonSerializer.Deserialize<UserDTO>(result);
            }
            else
            {
                result = response.ToString();
            }

            

          

          

            return FormatedResult;

        }

        public async static Task<SessionUserDataDTO> GatherUserDataFromLogin(string userName)
        {

            SessionUserDataDTO newData = new SessionUserDataDTO();
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true
            };



            string baseUrl = "http://login_api:80/api/";
            string PropUrl = "GetUserData/" + userName;
            string extendenUrl = baseUrl + "User/" + PropUrl;
            string result = "Req URL: " + extendenUrl + " - ";

            HttpClient client = new HttpClient(handler);
            HttpResponseMessage response = await client.GetAsync(extendenUrl);


            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();

            }
            else
            {
                result = response.ToString();
            }

            SessionUserDataDTO SessionUserResult = new SessionUserDataDTO();

            UserDTO FormatedResult = JsonSerializer.Deserialize<UserDTO>(result);

       ;
            SessionUserResult.Rank = FormatedResult.role;
            SessionUserResult.Guild = FormatedResult.guild;
            SessionUserResult.Online = false;

            foreach (string user in DataHandler.OnlineUsers)
            {
                if (user == FormatedResult.userName)
                {
                    SessionUserResult.Online = true;
                }
            }



            return SessionUserResult;




        


        }


        public static (bool,bool) HandleOnlineList(string UserNamer)
        {


          
                if (DataHandler.OnlineUsers.Contains(UserNamer))
                {
                    DataHandler.OnlineUsers.Remove(UserNamer);
                    return (true,false);
               }
                else
                {
                    DataHandler.OnlineUsers.Add(UserNamer);
                    return (true,true);
                }

            return (false,false);

        }

    }
}
