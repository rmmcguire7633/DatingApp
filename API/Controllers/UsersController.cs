using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.DataAccess;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepesitory;
        private readonly IMapper _map;
        
        public UsersController(IDatingRepository datingRepository, IMapper map)
        {
            _datingRepesitory = datingRepository;
            _map = map;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _datingRepesitory.GetUsers();

            var userToReturn = _map.Map<IEnumerable<UserForListDto>>(users);

            return Ok(userToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserWithId(int id)
        {
            var user = await _datingRepesitory.GetUser(id);

            var userToReturn = _map.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }
    }
}