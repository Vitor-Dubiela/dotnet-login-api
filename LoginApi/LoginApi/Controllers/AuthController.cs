using System;
using LoginApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoginApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User _user = new User();

        [HttpPost("register-user")]
        public ActionResult<User> RegisterUser([FromBody] UserDto userDto)
        {
            if (!ValidateUsername(userDto.Username))
                return BadRequest("The User name was not valid.");

            _user.Username = userDto.Username;
            _user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            return Ok(_user);
        }

        private static bool ValidateUsername(string username)
        {
            if (_user.Username.Equals(username))
                return false;
            return true;
        }

        [HttpPost("login")]
        public ActionResult<User> Login([FromBody] UserDto userDto)
        {
            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, _user.PasswordHash))
                return BadRequest("The password is invalid.");

            return Ok("You are connected.");
        }
    }
}

