using System.ComponentModel.DataAnnotations;
using System;

namespace Datingapp.API.Dtos
{
    public class UserToRegisterDto
    {
        public UserToRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Username {get; set;}
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        [StringLength(100,MinimumLength = 4 , ErrorMessage ="You must specify password betwween 100 and 4 character")]
        public string Password {get; set;}
        [Required]
        public string ConfirmPassword { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        
    }
}