using Badges.Core.Domain;
using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Core.Repositories;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Badges.Controllers
{
    /// <summary>
    /// Controller for the Badge class
    /// </summary>
    [Authorize(Roles=RoleNames.Student)]
    public class BadgeController : ApplicationController
    {
        public BadgeController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {

        }
    }
}
