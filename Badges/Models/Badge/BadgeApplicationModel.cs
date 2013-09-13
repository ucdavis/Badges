using System.Collections.Generic;
using Badges.Core.Domain;

namespace Badges.Models.Badge
{
    public class BadgeApplicationModel
    {
        public Core.Domain.Badge Badge { get; set; }
        public IList<BadgeCriteria> BadgeCriterias { get; set; }
        public IList<BadgeFulfillmentViewModel> Fulfillments { get; set; }
    }
}