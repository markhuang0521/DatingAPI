using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.helper;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DatingApp.Controllers
{
    [ServiceFilter(typeof(LongUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IDatingRepo _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        //api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var currentUser = await _repo.GetUser(currentUserId);

            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userParams.Gender == "male" ? "female" : "male";
            }

            var users = await _repo.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagation(users.CurrPage, users.PageSizes, users.TotalCount, users.totalPage);
            return Ok(usersToReturn);
        }
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            var userToReturn = _mapper.Map<UserForDetailDto>(user);

            return Ok(userToReturn);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _repo.GetUser(id);
            _mapper.Map(userDto, userFromRepo);

            if (await _repo.SaveAll())
            {
                return NoContent();
            }
            throw new Exception($"user {id} update fails");

        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> PostLike(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var like = await _repo.GetLike(id, recipientId);
            if (like != null)
            {
                return BadRequest("already liked the user");
            }
            if (_repo.GetUser(recipientId) == null)
            {
                return NotFound();
            }

            like = new Like()
            {
                LikeeId = recipientId,
                LikerId = id
            };
            _repo.Add<Like>(like);

            if (await _repo.SaveAll())
            {
                return Ok();
            }
            return BadRequest("failed to like to user");


        }




    }
}
