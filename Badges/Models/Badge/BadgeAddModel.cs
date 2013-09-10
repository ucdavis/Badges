using System.Collections.Generic;
using Badges.Core.Domain;

namespace Badges.Models.Badge
{
    public class BadgeAddModel
    {
        public Core.Domain.Badge Badge { get; set; }
        public IList<BadgeCategory> BadgeCategories { get; set; }
    }
}