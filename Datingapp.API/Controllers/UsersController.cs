using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Datingapp.API.Data;
using Datingapp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Datingapp.API.Controllers
{
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
        public async Task<IActionResult> GetUsers()
        {
           var users = await _repo.GetUsers();
           var usersForReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
           return Ok(usersForReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id) 
        {
           var user = await _repo.GetUser(id);
           var userForReturn = _mapper.Map<UserForDetailDto>(user);
           return Ok(userForReturn);
        }
        
    }
}