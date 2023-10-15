using SessionService.Data;
using SessionService.Model;

namespace SessionService.Class
{
    public static class LogicFunctions
    {

        public static SessionUserDataDTO GatherUserData(string userData)
        {

            SessionUserDataDTO newData = new SessionUserDataDTO();

            //To be made
            if (true)
            {
                newData.Guild = "TBD";
                newData.Rank = "TBD";
                return newData;
            } else
            {
                newData.Guild = "Error";
                newData.Rank = "Error";
                return null;
            }

        


        }


        public static bool HandleOnlineList(string UserNamer, bool newStatus)
        {


            if (newStatus == true)
            {
                if (DataHandler.OnlineUsers.Contains(UserNamer))
                {

                }
                else
                {
                    DataHandler.OnlineUsers.Add(UserNamer);
                    return true;
                }

            }
            else
            {
                if (DataHandler.OnlineUsers.Contains(UserNamer))
                {
                    DataHandler.OnlineUsers.Remove(UserNamer);
                    return true;
                }
                else
                {

                }
            }

            return false;

        }

    }
}
