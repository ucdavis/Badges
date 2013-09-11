using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Badges.Controllers;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using Badges.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Badges.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the BadgeCategory class
    /// </summary>
    public class BadgeCategoryController : ApplicationController
    {
        private readonly IFileService _fileService;
        //
        // GET: /Admin/BadgeCategory/
        public BadgeCategoryController(IRepositoryFactory repositoryFactory, IFileService fileService) : base(repositoryFactory)
        {
            _fileService = fileService;
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

            return View(viewModel);
        } 

        //
        // POST: /Admin/BadgeCategory/Create
        [HttpPost]
        public ActionResult Create(BadgeCategoryViewModel model)
        {
            var badgeCategoryToCreate = new BadgeCategory {Name = model.Name};

            var badgeImage = _fileService.Save(model.File, publicAccess: true);
            badgeCategoryToCreate.ImageUrl = badgeImage.Uri.AbsoluteUri;

            if (ModelState.IsValid)
            {
                RepositoryFactory.BadgeCategoryRepository.EnsurePersistent(badgeCategoryToCreate);

                Message = "BadgeCategory Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = BadgeCategoryViewModel.Create(Repository);
                viewModel.Name = model.Name;

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
            viewModel.Name = badgeCategory.Name;
            viewModel.ImageUrl = badgeCategory.ImageUrl;

			return View(viewModel);
        }
        
        //
        // POST: /Admin/BadgeCategory/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, BadgeCategoryViewModel model)
        {
            var badgeCategoryToEdit = RepositoryFactory.BadgeCategoryRepository.GetNullableById(id);

            if (badgeCategoryToEdit == null) return RedirectToAction("Index");

            badgeCategoryToEdit.Name = model.Name;
            
            if (ModelState.IsValid)
            {
                if (model.File != null) //replace file if we have a new one
                {
                    var badgeImage = _fileService.Save(model.File, publicAccess: true);
                    badgeCategoryToEdit.ImageUrl = badgeImage.Uri.AbsoluteUri;
                }

                RepositoryFactory.BadgeCategoryRepository.EnsurePersistent(badgeCategoryToEdit);

                Message = "BadgeCategory Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = BadgeCategoryViewModel.Create(Repository);
                viewModel.Name = model.Name;

                return View(viewModel);
            }
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
        [Required]
        [StringLength(140)]
        public string Name { get; set; }
        [Required]
        public HttpPostedFileBase File { get; set; }
	    public string ImageUrl { get; set; }

		public static BadgeCategoryViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");

		    var viewModel = new BadgeCategoryViewModel();
 
			return viewModel;
		}
	}
}
