using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Badges.Core.Domain;

namespace Badges.Models.Badge
{
    public class BadgeAddModel
    {
        [Required]
        [StringLength(64)] //Something short and descriptive
        public string Name { get; set; }
        [StringLength(140)]
        public string Description { get; set; }
        [Required]
        public BadgeCategory Category { get; set; }
        [Required]
        public string[] Criteria { get; set; }
        public IList<BadgeCategory> BadgeCategories { get; set; }
    }
}