using Microsoft.AspNetCore.Mvc;
using SessionService.Class;
using SessionService.Data;
using SessionService.Model;

namespace SessionService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[Action]")]
    public class SessionServiceController : ControllerBase
    {


        private readonly ILogger<SessionServiceController> _logger;

        public SessionServiceController(ILogger<SessionServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Login/{userName}")]
        public ActionResult GetLogin(string userName)
        {

            (bool,bool) suscces = LogicFunctions.HandleOnlineList(userName);

            if (suscces == (true, true))
            {
                return Ok(userName + " is now Online");
            } else if (suscces == (true, false)) { 
                 return Ok(userName + " is now Offline");
                
             } else
            {
                return NotFound("User Dont Exist");
            }

        }


            [HttpGet("Data/{userName}")]
            public ActionResult<string>Get(string userName)
            {

                string Result = LogicFunctions.GatherUserData(userName);



                if (Result == null)
                {
                    return NotFound();

                } else { 
                    return Ok(Result);
                }

            }
            
        }
}