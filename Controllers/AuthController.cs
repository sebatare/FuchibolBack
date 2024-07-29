using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fuchibol.ChatService.Models;

namespace Fuchibol.ChatService.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly List<User> _users = new List<User>
		{
			new User { Id = "1", Email = "user@example.com", Password = "password", Name = "User1", Age = 30 },
			new User { Id = "2", Email = "resu@example.com", Password = "password", Name = "User2", Age = 27 }
			// Agrega más usuarios según sea necesario
		};

		public AuthController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] UserLogin UserLogin)
		{
			var user = _users.Find(u => u.Email == UserLogin.Email && u.Password == UserLogin.Password);
			if (user != null)
			{	
				var token = GenerateJwtToken(user);
				Console.WriteLine("token desde el controlador");
				Console.WriteLine(token);
				return Ok(new { token });
			}

			return Unauthorized();
		}

		private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email), // Incluye el email como claim
                new Claim("UserId", user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

	}
}
