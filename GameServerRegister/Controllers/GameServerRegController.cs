using GameServerRegister.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameServerRegister.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameServerRegController : ControllerBase
    {
        public static Dictionary<ServerIpDTO, DateTime> ServerRegister = new Dictionary<ServerIpDTO, DateTime>();

        public static HttpClient SetupHttpClient(string ip)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://"+ip);
            client.Timeout = TimeSpan.FromSeconds(5);
            return client;
        }

        [HttpGet("GetServer")]
        public async Task<ActionResult<ServerIpDTO>> GetServer()
        {
            foreach (var dto in ServerRegister)
            {
                Console.WriteLine($"ip: {dto.Key.ip}:{dto.Key.port}, at time: {dto.Value}");
                if((DateTime.Now-dto.Value).TotalSeconds > 30)
                {
                    Console.WriteLine($"ip: {dto.Key.ip}:{dto.Key.port}, passed the check");
                    ServerRegister[dto.Key] = DateTime.Now;
                    return Ok(dto.Key);
                }
            }
            return Ok(new ServerIpDTO() { ip="", port=0 });
        }

        [HttpPost("Full")]
        public ActionResult StillFull([FromBody] ServerIpDTO serverInfoDTO)
        {
            serverInfoDTO = ServerRegister.Keys.DistinctBy(other => other.ip == serverInfoDTO.ip && other.port == serverInfoDTO.port).FirstOrDefault();
            if (serverInfoDTO != null)
            {
                ServerRegister[serverInfoDTO] = DateTime.Now;
                //Console.WriteLine($"Gameserver with ip {serverInfoDTO.ip}:{serverInfoDTO.port} is still occupied, updating timestamp");
                return Ok($"Gameserver with ip {serverInfoDTO.ip}:{serverInfoDTO.port} is still occupied, updating timestamp");
            }
            else
            {
                return BadRequest($"Gameserver with ip {serverInfoDTO.ip}:{serverInfoDTO.port} is not registered");
            }
        }

        [HttpPost("Register")]
        public ActionResult RegisterServer([FromBody] ServerIpDTO serverInfoDTO)
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
            return Ok("Gameserver has been registered");
        }
    }
}
