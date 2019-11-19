using System;
namespace DatingApp.Dtos
{
    public class MessageCreateDto
    {

        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime MessageSent { get; set; }
        public string Content { get; set; }

        public MessageCreateDto()
        {
            MessageSent = DateTime.Now;

        }


    }
}

