using Microsoft.AspNetCore.Mvc;
using SessionService.Class;
using SessionService.Data;
using SessionService.Model;

namespace SessionService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SessionServiceController : ControllerBase
    {
 

        private readonly ILogger<SessionServiceController> _logger;

        public SessionServiceController(ILogger<SessionServiceController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult Post(string userName, bool Status)
        {
        
            if(Status == true)
            {
                if (DataHandler.OnlineUsers.Contains(userName)){

                } else
                {
                    DataHandler.OnlineUsers.Add(userName);
                    return Ok(userName + " is now Online");
                }
             
            } else
            {
                if (DataHandler.OnlineUsers.Contains(userName))
                {
                    DataHandler.OnlineUsers.Remove(userName);
                    return Ok(userName + " is now Offline");
                }
                else
                {
                    
                }
            }
            return NotFound();
        }


            [HttpGet]
        public ActionResult<SessionUserDataDTO> Get(string userName)
        {

            SessionUserDataDTO Result = LogicFunctions.GatherUserData(userName);

            if(Result != null)
            {
                foreach (string name in DataHandler.OnlineUsers)
                {
                    if (name == userName)
                    {
                        Result.Online = true;
                        break;
                    }
                    else
                    {
                        Result.Online = false;
                    }
                }

            }

            if (Result == null)
            {
                return NotFound();
              
            } else { 
                return Ok(Result);
            }

        }
    }
}