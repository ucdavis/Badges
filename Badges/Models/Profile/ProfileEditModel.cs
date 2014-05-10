using System.Collections.Generic;
using Badges.Core.Domain;

namespace Badges.Models.Profile
{
    public class ProfileEditModel
    {
        public Core.Domain.Profile Profile { get; set; }
        public IList<Role> Roles { get; set; }
        public bool isInstructor { get; set; }
    }
}