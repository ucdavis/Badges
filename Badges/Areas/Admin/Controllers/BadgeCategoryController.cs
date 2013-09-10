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
    /// Controller for the BadgeCategory class
    /// </summary>
    public class BadgeCategoryController : ApplicationController
    {
        //
        // GET: /Admin/BadgeCategory/
        public BadgeCategoryController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            var badgeCategoryList = RepositoryFactory.BadgeCategoryRepository.Queryable;

            return View(badgeCategoryList.ToList());
        }

        //
        // GET: /Admin/BadgeCategory/Create
        public ActionResult Create()
        {
			var viewModel = BadgeCategoryViewModel.Create(Repository);

            //TODO: Remove hacky way of getting image and use file upload
            viewModel.BadgeCategory.ImageUrl = Url.Action("Index", "Home", new { area = string.Empty }, "http") + Url.Content("~/Content/images/climbingbadge.png");

            return View(viewModel);
        } 

        //
        // POST: /Admin/BadgeCategory/Create
        [HttpPost]
        public ActionResult Create(BadgeCategory badgeCategory)
        {
            var badgeCategoryToCreate = new BadgeCategory();

            TransferValues(badgeCategory, badgeCategoryToCreate);

            if (ModelState.IsValid)
            {
                RepositoryFactory.BadgeCategoryRepository.EnsurePersistent(badgeCategoryToCreate);

                Message = "BadgeCategory Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = BadgeCategoryViewModel.Create(Repository);
                viewModel.BadgeCategory = badgeCategory;

                return View(viewModel);
            }
        }

        //
        // GET: /Admin/BadgeCategory/Edit/5
        public ActionResult Edit(Guid id)
        {
            var badgeCategory = RepositoryFactory.BadgeCategoryRepository.GetNullableById(id);

            if (badgeCategory == null) return RedirectToAction("Index");

			var viewModel = BadgeCategoryViewModel.Create(Repository);
			viewModel.BadgeCategory = badgeCategory;

			return View(viewModel);
        }
        
        //
        // POST: /Admin/BadgeCategory/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, BadgeCategory badgeCategory)
        {
            var badgeCategoryToEdit = RepositoryFactory.BadgeCategoryRepository.GetNullableById(id);

            if (badgeCategoryToEdit == null) return RedirectToAction("Index");

            TransferValues(badgeCategory, badgeCategoryToEdit);

            if (ModelState.IsValid)
            {
                RepositoryFactory.BadgeCategoryRepository.EnsurePersistent(badgeCategoryToEdit);

                Message = "BadgeCategory Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = BadgeCategoryViewModel.Create(Repository);
                viewModel.BadgeCategory = badgeCategory;

                return View(viewModel);
            }
        }
        
        //
        // GET: /Admin/BadgeCategory/Delete/5 
        public ActionResult Delete(Guid id)
        {
			var badgeCategory = RepositoryFactory.BadgeCategoryRepository.GetNullableById(id);

            if (badgeCategory == null) return RedirectToAction("Index");

            return View(badgeCategory);
        }

        //
        // POST: /Admin/BadgeCategory/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, BadgeCategory badgeCategory)
        {
			var badgeCategoryToDelete = RepositoryFactory.BadgeCategoryRepository.GetNullableById(id);

            if (badgeCategoryToDelete == null) return RedirectToAction("Index");

            RepositoryFactory.BadgeCategoryRepository.Remove(badgeCategoryToDelete);

            Message = "BadgeCategory Removed Successfully";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(BadgeCategory source, BadgeCategory destination)
        {
            destination.Name = source.Name;
            destination.ImageUrl = source.ImageUrl;
        }

    }

	/// <summary>
    /// ViewModel for the BadgeCategory class
    /// </summary>
    public class BadgeCategoryViewModel
	{
		public BadgeCategory BadgeCategory { get; set; }
 
		public static BadgeCategoryViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new BadgeCategoryViewModel {BadgeCategory = new BadgeCategory()};
 
			return viewModel;
		}
	}
}
