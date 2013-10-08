using System.Collections.Generic;
using System.ComponentModel;

namespace LinkLogger.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [DisplayName("User name")]
        public string UserName { get; set; }
    }

    public class EditRoleMembersViewModel
    {
        public IEnumerable<UserViewModel> CurrentMembers { get; set; }

        public IEnumerable<UserViewModel> AvailableUsers { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
    }
}