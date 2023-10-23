using Login.Model;
using System;
using System.IO;
using System.Net;

namespace Login.Class
{

    public enum APIRequestType { Data, login}

    public static class LogicFunctions
    {

        public static string RequestToSessionService(UserDto user, APIRequestType type)
        {
   
            string response = CallGetString(user, type);
            return response;
        }

 

        public static string CallGetString(UserDto user, APIRequestType type)
        {
            Task<string> task = null; 

            if (type == APIRequestType.Data)
            {
                 task = RequestToSessionServiceUserData(user.UserName);

                task.Wait();
            }

            if (type == APIRequestType.login)
            {
                 task = RequestToSessionServiceSetUserOnline(user.UserName);

                task.Wait();
            }


            string result = task.Result;
            return result;
        }


        /// <summary>
        /// Login Request Change in SessionService, To see if User is Online. 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task<string> RequestToSessionServiceUserData(string userName)
        {

            HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true
            };

 

            string baseUrl = "http://session_service_api:80/api";
            string PropUrl = "Data/" + userName;
            string extendenUrl = baseUrl + "SessionService/" + PropUrl;
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


        public static async Task<string> RequestToSessionServiceSetUserOnline(string userName)
        {

            HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true
            };



            string baseUrl = "http://session_service_api:80/api/";
            string PropUrl = "GetLogin/Login/" + userName;
            string extendenUrl = baseUrl + "SessionService/" + PropUrl;
            string result = "Form: " + extendenUrl +" - ";

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

    }
}
