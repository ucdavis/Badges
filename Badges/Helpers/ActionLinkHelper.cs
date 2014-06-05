using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Controllers;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Badges.Helpers
{
    public static class ActionLinkHelper
    {
        public static string ActionLink(string endpoint, string linktext)
        {
            return String.Format("<a href=\"{0}\" class=\"btn action-link\">{1}</a>", endpoint, linktext);
        }
    }
}
