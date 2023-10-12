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

    }
}
