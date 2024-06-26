﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SessionService.Class;
using SessionService.Data;
using SessionService.Model;
using System.Linq;

namespace SessionService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MsgChatController : ControllerBase
    {

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


        [HttpPost("PrivateMSG"), Authorize(Roles = "Admin, User")]
        public ActionResult<string> SendPrivateMessage(MessageForm form)
        {
          
            if (DataHandler.OnlineUsers.Contains(form.Target))
            {
                ConnectionManager.ProducePrivateMsg(form.Message, form.Target);
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
        [Route("GuildMSG"), Authorize(Roles = "Admin, User")]
        public ActionResult<string> SendGuildMessage(MessageForm form)
        {
            ConnectionManager.ProduceGuildMsg(form.Message, form.Target);
            return Ok("Guild Message from " + form.User + ": " + form.Message); 
        }


        [HttpPost]
        [Route("PublicMSG"), Authorize(Roles = "Admin")]
        public ActionResult<string> SendPublicMessage(MessageForm form)
        {
            ConnectionManager.ProducePublicMsg(form.Message);
            return Ok("Global: " + form.Message);
        }
    }
}
