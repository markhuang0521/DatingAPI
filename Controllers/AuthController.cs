using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Controllers
{
    //localhost:5000/api/
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo _auth;
        private readonly IConfiguration _config;

        //DI for Iauth
        public AuthController(IAuthRepo repo, IConfiguration config)
        {
            _auth = repo;
            _config = config;
        }

        //get all user 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _auth.GetUsers();
            return Ok(users);
        }


        //register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userDto)
        {
            //validate user input
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            userDto.UserName = userDto.UserName.ToLower();
            if (await _auth.UserExist(userDto.UserName))
            {
                return BadRequest("UerName already exists");
            }
            var userToCreate = new User
            {
                UserName = userDto.UserName
            };
            var createdUser = await _auth.Register(userToCreate, userDto.Password);

            //return CreatedAtRoute()

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userDto)
        {

            //getting the existing user from database
            var existingUser = await _auth.Login(userDto.UserName.ToLower(), userDto.Password);
            if (existingUser == null)
            {
                return Unauthorized();
            }

            //create new claims that store the user id and username inside of the token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,existingUser.Id.ToString()),
                new Claim(ClaimTypes.Name, existingUser.UserName)
            };

            //create key and tokens 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            //token description
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credential
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });






        }

    }


}