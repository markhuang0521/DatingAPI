using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "password must be 6 character or greater")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }

        [Required]

        public string KnownAs { get; set; }

        [Required]

        public DateTime DateOfBirth { get; set; }

        [Required]

        public string City { get; set; }

        [Required]

        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActive { get; set; }

        public UserRegisterDto()
        {
            CreatedAt = DateTime.Now;
            LastActive = DateTime.Now;

        }
    }
}
