using System.ComponentModel.DataAnnotations;

namespace Datingapp.API.Dtos
{
    public class UserToRegisterDto
    {
        [Required]
        public string Username {get; set;}

        [Required]
        [StringLength(100,MinimumLength = 4 , ErrorMessage ="You must specify password betwween 100 and 4 character")]
        public string Password {get; set;}
        
    }
}