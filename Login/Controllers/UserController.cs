using Login.Class;
using Login.Data;
using Login.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Login.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]

    public class UserController : ControllerBase
    {
 
        public static User user = new User();
        private readonly IConfiguration _configuration;

        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
        }


        [HttpPost("Login")]
        public ActionResult<string> ValidateUser(LoginObject loginData)
        {
         
            foreach(User user in DataHandler.UserList)
            {
                if(user.UserName == loginData.UserName)
                {
                    if(!BCrypt.Net.BCrypt.Verify(loginData.Password, user.PasswordHash))
                    {

                        return BadRequest("Wrong Password");
                    }
                    else
                    {
                        //LogicFunctions.UpdateDataOnSessionService(user);
                        string token = CreateWebToken(user);
                        return Ok(token);
                    }
                }
            }

            return BadRequest("No User Match");

        }
        

        [HttpGet]
        public ActionResult<User> GetUserData(string name)
        {
            User selectedUser = null;

            foreach (User user in DataHandler.UserList)
            {
                if (user.UserName == name)
                {
                    
                        selectedUser = user;
                        return Ok(selectedUser);
                    
                }
            }

            return NotFound();

        }


        [HttpGet]
        public ActionResult<string> GetAllUsers()
        {

            //LogicFunctions.UpdateDataOnSessionService();
            return Ok(LogicFunctions.UpdateDataOnSessionService());

        
      
        }
     

        private string CreateWebToken(User user)
        {
            List<Claim> claims = new List<Claim> { 
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        
    }
}