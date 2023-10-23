using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SessionService.Class;
using SessionService.Interface;

namespace SessionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult> SendMessage([FromBody] string message)
        {
            _messagePublisher.SendMessage(message);
            return Ok();
        }
    }
}
