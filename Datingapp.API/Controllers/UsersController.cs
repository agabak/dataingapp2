using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Datingapp.API.Data;
using Datingapp.API.Dtos;
using Datingapp.API.Helpers;
using Datingapp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Datingapp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivities))]
    [Authorize]
    [Route("api/{controller}")]
    [ApiController]
    public class UsersController:ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
           var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
           var userFromRepo = await _repo.GetUser(currentUserId);

           if(string.IsNullOrEmpty(userParams.Gender)) 
           {
               userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
           }
           var users = await _repo.GetUsers(userParams);
           var usersForReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

           Response.AddPagination(users.CurrentPage,userParams.PageSize,users.TotalCount, users.TotalPages);
           return Ok(usersForReturn);
        }

        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id) 
        {
           var user = await _repo.GetUser(id);
           var userForReturn = _mapper.Map<UserForDetailDto>(user);
           return Ok(userForReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDtos userForUpdateDtos)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _repo.GetUser(id);

            _mapper.Map(userForUpdateDtos, userFromRepo);

            if(await _repo.SaveAll()) {
                return NoContent();
            }
            return BadRequest();
        }

        [HttpPost("{id}/like/{recipientId}")]  
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            
            var like = await _repo.GetLike(id, recipientId);
            if(like != null)
                return BadRequest("Your already like this member");

            if( await _repo.GetUser(recipientId) == null) {
                return  NotFound();
            }

            like = new Like { LikerId = id, LikeeId = recipientId};
            _repo.Add<Like>(like);
            if(await _repo.SaveAll()) {
                return Ok();
            }
            return  BadRequest("Failed to like user");
        }
    }
}