using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SessionService.Class;
using SessionService.Interface;

namespace SessionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MsgChatController : ControllerBase
    {
        private readonly IMessageProducer _messagePublisher = new RabbitMQProducer();
        public MsgChatController(RabbitMQProducer messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok(ConnectionManager.static_message);
        }


        [HttpPost]
        [Route("PrivateMSG")]
        public async Task<ActionResult> SendPrivateMessage([FromBody] string message)
        {
            _messagePublisher.SendMessage(message);
            return Ok();
        }


        [HttpPost]
        [Route("GuildMSG")]
        public async Task<ActionResult> SendGuildMessage([FromBody] string message)
        {
            _messagePublisher.SendMessage(message);
            return Ok();
        }


        [HttpPost]
        [Route("PublicMSG")]
        public async Task<ActionResult> SendPublicMessage([FromBody] string message)
        {
            _messagePublisher.SendMessage(message);
            return Ok();
        }
    }
}
