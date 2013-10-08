using System.ComponentModel.DataAnnotations;

namespace LinkLogger.Models
{
    public class RoleViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Id { get; set; }
    }
}