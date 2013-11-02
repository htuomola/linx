using System.Collections.Generic;

namespace LinkLogger.Models
{
    public class EditRoleMembersViewModel
    {
        public IEnumerable<UserViewModel> CurrentMembers { get; set; }

        public IEnumerable<UserViewModel> AvailableUsers { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
    }
}