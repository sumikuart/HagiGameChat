using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SessionService.Class;
using SessionService.Data;
using SessionService.Interface;
using SessionService.Model;
using System.Linq;

namespace SessionService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MsgChatController : ControllerBase
    {
        private readonly IMessageProducer _messagePublisher = new RabbitMQProducer();
        private readonly ILogger<MsgChatController> _logger;
        private readonly IConfiguration _configuration;

 
        public MsgChatController(ILogger<MsgChatController> logger, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok(ConnectionManager.static_message);
        }


        [HttpPost("PrivateMSG")]
        public ActionResult<string> SendPrivateMessage(MessageForm form)
        {
          
            if (DataHandler.OnlineUsers.Contains(form.Target))
            {
                return Ok("Private msg to :" + form.Target + " - " + form.Message);
            }
            else
            {
                bool userExist = LogicFunctions.UserOnlineData(form.Target);

                if (userExist)
                {
                    return Ok(form.Target + " Is Offline");
                } else
                {
                    return NotFound(form.Target + " Dont Exist");
                }
           
            }
                 
        }


        [HttpPost]
        [Route("GuildMSG")]
        public ActionResult<string> SendGuildMessage(MessageForm form)
        {

            return Ok("Guild Message from " + form.User + ": " + form.Message); 
        }


        [HttpPost]
        [Route("PublicMSG"), Authorize(Roles = "Admin")]
        public ActionResult<string> SendPublicMessage(string message)
        {

            return Ok("Global: " + message);
        }
    }
}
