using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Datingapp.API.Data;
using Datingapp.API.Dtos;
using Datingapp.API.Helpers;
using Datingapp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Datingapp.API.Controllers
{

    [AllowAnonymous]
    [Route("api/{controller}")]
    [ApiController]
    public class AuthController:ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public SignInManager<User> _signInManager { get; }

        public AuthController(IAuthRepository repository, IConfiguration config,
                              IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _mapper = mapper;
        }

          [HttpPost ("register")]
        public async Task<IActionResult> Register ([FromBody]UserToRegisterDto userToRegisterDto) 
        {
            if(!string.Equals(userToRegisterDto.Password, userToRegisterDto.ConfirmPassword)) {
                return BadRequest("Password must match");
            }
            userToRegisterDto.Username = userToRegisterDto.Username.ToLower ();
         //   if (await _repository.UserExist (userToRegisterDto.Username)) return BadRequest ("username already exists");

            var userToCreate = _mapper.Map<User>(userToRegisterDto);

         //   var createdUser = await _repository.Register (userToCreate, userToRegisterDto.Password);
         //   var userToReturn = _mapper.Map<UserForDetailDto>(createdUser);

            return null; // CreatedAtRoute("GetUser", new { Controller = "Users", id = createdUser.Id}, userToReturn);
        }

         [HttpPost ("login")]
        public async Task<IActionResult> Login (UserForLoginDto userForLoginDto) 
        {
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if(result.Succeeded) {

               var  appUser = await _userManager.Users
                                          .Include(p => p.Photos)
                                          .FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());

                var userToReturn = _mapper.Map<UserForListDto>(appUser);

            return Ok (new {
                token = GenerateJwtToken(appUser),
                user = userToReturn
            });
            }
            return Unauthorized();
        }

        private string GenerateJwtToken(User user)
        {
            var claim = new [] {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString ()),
                new Claim (ClaimTypes.Name, user.UserName)
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

            return tokenHandler.WriteToken(token);
        }     
    }
}