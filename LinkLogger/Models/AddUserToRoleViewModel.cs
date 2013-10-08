using System.ComponentModel.DataAnnotations;

namespace LinkLogger.Models
{
    public class AddUserToRoleViewModel
    {
        [Required]
        public string RoleId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}