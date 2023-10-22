using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameServerRegister.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameServerRegController : ControllerBase
    {
        public static Dictionary<ServerInfoDTO, DateTime> ServerRegister = new Dictionary<ServerInfoDTO, DateTime>();

        public static HttpClient SetupHttpClient(string ip)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://"+ip);
            client.Timeout = TimeSpan.FromSeconds(5);
            return client;
        }

        [HttpGet("GetServer")]
        public async Task<ActionResult<ServerInfoDTO>> GetServer()
        {
            foreach (var dto in ServerRegister)
            {
                if((dto.Value-DateTime.Now).TotalSeconds > 30)
                {
                    return Ok(dto.Key);
                }
            }
            return Ok(new ServerInfoDTO() { ip="", port=0 });
        }

        [HttpPost("Full")]
        public ActionResult StillFull([FromBody] ServerInfoDTO serverInfoDTO)
        {
            if (ServerRegister.ContainsKey(serverInfoDTO))
            {
                ServerRegister[serverInfoDTO] = DateTime.Now;
                Console.WriteLine($"Gameserver with ip {serverInfoDTO.ip}:{serverInfoDTO.port} is still occupied, updating timestamp");
                return Ok($"Gameserver with ip {serverInfoDTO.ip}:{serverInfoDTO.port} is still occupied, updating timestamp");
            }
            else
            {
                return BadRequest($"Gameserver with ip {serverInfoDTO.ip}:{serverInfoDTO.port} is not registered");
            }
        }

        [HttpPost("Register")]
        public ActionResult RegisterServer([FromBody]ServerInfoDTO serverInfoDTO)
        {
            if (ServerRegister.ContainsKey(serverInfoDTO))
            {
                ServerRegister[serverInfoDTO] = DateTime.Now.AddMinutes(-1);
                Console.WriteLine($"Gameserver with ip {serverInfoDTO.ip}:{serverInfoDTO.port} already exsists, but has been marked as potentiely open");
            } else
            {
                ServerRegister.Add(serverInfoDTO, DateTime.Now.AddMinutes(-1));
                Console.WriteLine($"Gameserver with ip {serverInfoDTO.ip}:{serverInfoDTO.port} has been registered");
            }
            return Ok($"Gameserver with ip {serverInfoDTO.ip}:{serverInfoDTO.port} has been registered");
        }
    }
}
