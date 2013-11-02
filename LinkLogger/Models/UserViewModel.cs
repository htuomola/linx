using System.ComponentModel;

namespace LinkLogger.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [DisplayName("User name")]
        public string UserName { get; set; }
    }
}