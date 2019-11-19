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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IDatingRepo _repo;
        private readonly IMapper _mapper;


        public MessagesController(IDatingRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }



        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo == null)
            {
                return NotFound();
            }
            return Ok(messageFromRepo);



        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            messageParams.UserId = userId;

            var messageFromRepo = await _repo.GetMessagesForUser(messageParams);
            var messages = _mapper.Map<IEnumerable<MessageReturnDto>>(messageFromRepo);

            Response.AddPagation(messageFromRepo.CurrPage, messageFromRepo.PageSizes,
                messageFromRepo.TotalCount, messageFromRepo.totalPage);
            return Ok(messages);

        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessagesThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var messageFromRepo = await _repo.GetMessagesThread(userId, recipientId);

            var messageThread = _mapper.Map<IEnumerable<MessageReturnDto>>(messageFromRepo);
            return Ok(messageThread);

        }


        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageCreateDto messageCreateDto)
        {
            var sender = await _repo.GetUser(userId);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageCreateDto.SenderId = userId;
            var receipient = await _repo.GetUser(messageCreateDto.RecipientId);

            if (receipient == null)
            {
                return BadRequest("receipient not found");
            }
            var message = _mapper.Map<Message>(messageCreateDto);

            _repo.Add(message);
            var messageToReturn = _mapper.Map<MessageReturnDto>(message);


            if (await _repo.SaveAll())
                return CreatedAtRoute("GetMessage", new { id = message.Id }, messageToReturn);
            throw new Exception("fail to creat message");

        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);
            if (messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDelete = true;
            }
            if (messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.RecipientDelete = true;
            }
            if (messageFromRepo.SenderDelete && messageFromRepo.RecipientDelete)
            {
                _repo.Delete(messageFromRepo);
            }

            if (await _repo.SaveAll())
            {
                return NoContent();
            }
            throw new Exception("message fail to delete");

        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);
            if (messageFromRepo.RecipientId != userId)
                return Unauthorized();

            messageFromRepo.IsRead = true;
            messageFromRepo.DateRead = DateTime.Now;

            await _repo.SaveAll();
            return NoContent();


        }


    }
}
