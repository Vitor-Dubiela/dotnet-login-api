using System;
using LoginApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoginApi.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		public static List<User> _users = new List<User>();

		[HttpPost("register-user")]
		public async Task<ActionResult<User>> RegisterUser([FromBody] UserDto userDto)
		{
			if (!ValidateUsername(userDto.Username))
				return BadRequest("The User name was not valid.");

			return Ok();
		}

		private static bool ValidateUsername(string username)
		{
			foreach (var user in _users)
			{
				if (user.Username.Equals(username))
					return false;
			}
			return true;
		}
	}
}

