using System.ComponentModel.DataAnnotations;

namespace emlak.api.DTOs
{
    public class AssignRoleDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}