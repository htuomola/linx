using System.ComponentModel.DataAnnotations;

namespace LinkLogger.Controllers.Api
{
    public class LinkModel
    {
        public int? Id { get; set; }
        
        [Required]
        public string Url { get; set; }
        
        [Required]
        public string User { get; set; }
        
        [Required]
        public string Channel { get; set; }
        
        public System.DateTime? PostedAt { get; set; }
    }
}