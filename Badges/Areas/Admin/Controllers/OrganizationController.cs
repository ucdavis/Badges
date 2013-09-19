using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Controllers;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Badges.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the Organization class
    /// </summary>
    [Authorize(Roles=RoleNames.Administrator)]
    public class OrganizationController : ApplicationController
    {
        //
        // GET: /Admin/Organization/
        public OrganizationController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            var organizationList = RepositoryFactory.OrganizationRepository.Queryable;

            return View(organizationList.ToList());
        }

        //
        // GET: /Admin/Organization/Create
        public ActionResult Create()
        {
			var viewModel = OrganizationViewModel.Create(Repository);
            
            return View(viewModel);
        }

        //
        // POST: /Admin/Organization/Create
        [HttpPost]
        public ActionResult Create(Organization organization)
        {
            var organizationToCreate = new Organization();

            TransferValues(organization, organizationToCreate);

            if (ModelState.IsValid)
            {
                RepositoryFactory.OrganizationRepository.EnsurePersistent(organizationToCreate);

                Message = "Organization Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = OrganizationViewModel.Create(Repository);
                viewModel.Organization = organization;

                return View(viewModel);
            }
        }

        //
        // POST: /Admin/Organization/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, Organization organization)
        {
			var organizationToDelete = RepositoryFactory.OrganizationRepository.GetNullableById(id);

            if (organizationToDelete == null) return RedirectToAction("Index");

            RepositoryFactory.OrganizationRepository.Remove(organizationToDelete);

            Message = "Organization Removed Successfully";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Organization source, Organization destination)
        {
            destination.Name = source.Name;
        }

    }

	/// <summary>
    /// ViewModel for the Organization class
    /// </summary>
    public class OrganizationViewModel
	{
		public Organization Organization { get; set; }
 
		public static OrganizationViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new OrganizationViewModel {Organization = new Organization()};
 
			return viewModel;
		}
	}
}
