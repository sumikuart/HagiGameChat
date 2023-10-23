using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using SessionService.Data;
using SessionService.Model;
using System.Text;
using System.Threading.Tasks;

namespace SessionService.Class
{
    public static class LogicFunctions
    {


        public static string GatherUserData(string userData)
        {
               
           Task<string> task = null;
          
            task = GatherUserDataFromLogin(userData);

            task.Wait();



            string result = task.Result;
            return result;
        }



        public async static Task<string> GatherUserDataFromLogin(string userData)
        {

            SessionUserDataDTO newData = new SessionUserDataDTO();
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true
            };



            string baseUrl = "http://login_api:80/api/";
            string PropUrl = "GetUserData/" + userData;
            string extendenUrl = baseUrl + "User/" + PropUrl;
            string result = "Req URL: " + extendenUrl + " - ";

            HttpClient client = new HttpClient(handler);
            HttpResponseMessage response = await client.GetAsync(extendenUrl);


            if (response.IsSuccessStatusCode)
            {
                result += await response.Content.ReadAsStringAsync();

            }
            else
            {
                result += response.ToString();
            }


            return result;




        


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
