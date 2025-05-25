using System.ComponentModel.DataAnnotations;

namespace emlak.api.DTOs
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
