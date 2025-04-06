using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Dtos;
using WebAPI.Interfaces;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUnitOfWork uow;
        private readonly IConfiguration config;

        public AccountController(IUnitOfWork uow, IConfiguration config)
        {
            this.uow = uow;
            this.config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await uow.UserRepository.Authenticate(loginDto.Username, loginDto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var loginRes = new LoginResDto
            {
                Username = user.Username,
                Token = CreateJWT(user)
            };

            return Ok(loginRes);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginDto loginDto)
        {
            if (await uow.UserRepository.UserAlreadyExists(loginDto.Username))
            {
                return BadRequest("Username already exists");
            }

            uow.UserRepository.Register(loginDto.Username, loginDto.Password);
            await uow.SaveAsync();
            return StatusCode(201);
        }

        private string CreateJWT(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
