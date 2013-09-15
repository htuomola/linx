using System.ComponentModel.DataAnnotations;

namespace LinkLogger.Models
{
    public class LinkModel
    {
        public int? Id { get; set; }
        
        [Required]
        [MinLength(10)]
        public string Url { get; set; }
        
        [Required]
        [MinLength(2)]
        public string User { get; set; }
        
        [Required]
        [MinLength(2)]
        public string Channel { get; set; }

        public string Title { get; set; }

        public System.DateTime? PostedAt { get; set; }
    }
}