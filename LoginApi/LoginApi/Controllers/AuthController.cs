using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LoginApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User _user = new User();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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
            if (!userDto.Username.Equals(_user.Username))
                return BadRequest("The User name is invalid.");

            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, _user.PasswordHash))
                return BadRequest("The password is invalid.");

            var token = CreateToken(_user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Authentication:Token").Value!));

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

