﻿using System;

namespace Badges.Models.Badge
{
    public class BadgeFulfillmentViewModel
    {
        public Guid CriteriaId { get; set; }
        public Guid WorkId { get; set; }

        public string Details { get; set; }
        public string WorkType { get; set; }
    }
}