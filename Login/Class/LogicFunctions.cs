using Login.Model;
using System;
using System.IO;
using System.Net;

namespace Login.Class
{
    public static class LogicFunctions
    {

        public static string UpdateDataOnSessionService()
        {

            
            string response = CallGetString(); 

   

            return response;
        }

        public static string CallGetString()
        {

            var task = RequestFromLoginJWT();

            task.Wait();

            string result = task.Result;
            return result;
        }


        public static async Task<string> RequestFromLoginJWT()
        {

            HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true
            };

 

            string baseUrl = "http://host.docker.internal:80/api/";
            string extendenUrl = baseUrl + "WeatherForecast";
            string result = "";

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


            return result;

        }

    }
}
