using Login.Class;
using Login.Data;
using Login.Model;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Login.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
 

        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public ActionResult<bool> ValidateUser(string name, string password)
        {
         
            foreach(User user in DataHandler.UserList)
            {
                if(user.UserName == name)
                {
                    if(user.Password == password)
                    {
                        LogicFunctions.UpdateDataOnSessionService();
                        return Ok(true);
                    }
                }
            }

            return Ok(false);

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
        public ActionResult<List<User>> GetAllUsers()
        {

            return Ok(DataHandler.UserList);

        
      
        }
        
    }
}