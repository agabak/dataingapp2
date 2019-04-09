using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Datingapp.API.Data;
using Datingapp.API.Dtos;
using Datingapp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Datingapp.API.Controllers
{

    [Route("api/{controller}")]
    [ApiController]
    public class AuthController:ControllerBase
    {

        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }

          [HttpPost ("register")]
        public async Task<IActionResult> Register (UserToRegisterDto userToRegisterDto) 
        {
            userToRegisterDto.Username = userToRegisterDto.Username.ToLower ();
            if (await _repository.UserExist (userToRegisterDto.Username)) return BadRequest ("username already exists");

            var userToCreate = new User { Username = userToRegisterDto.Username };
            userToCreate = await _repository.Register (userToCreate, userToRegisterDto.Password);
            return StatusCode (201);
        }

         [HttpPost ("login")]
        public async Task<IActionResult> Login (UserForLoginDto userForLoginDto) 
        {
            var user = await _repository.Login (userForLoginDto.Username.ToLower(), userForLoginDto.Password);
            if (user == null) return Unauthorized ();

            var claim = new [] {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString ()),
                new Claim (ClaimTypes.Name, user.Username)
            };
            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            var tokenDescriptor = new SecurityTokenDescriptor 
                                    {
                                        Subject = new ClaimsIdentity(claim),
                                        Expires = DateTime.Now.AddDays(1),
                                        SigningCredentials = creds
                                    };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok (new {
                token = tokenHandler.WriteToken(token)
            });
        }
         
    }
}