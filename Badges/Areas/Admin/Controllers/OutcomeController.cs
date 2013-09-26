using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Badges.Controllers;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Badges.Services;

namespace Badges.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the Outcome class
    /// </summary>
    public class OutcomeController : ApplicationController
    {
        private readonly IFileService _fileService;

        //
        // GET: /Admin/Outcome/
        public OutcomeController(IRepositoryFactory repositoryFactory, IFileService fileService) : base(repositoryFactory)
        {
            _fileService = fileService;
        }

        public ActionResult Index()
        {
            var outcomeList = RepositoryFactory.OutcomeRepository.Queryable;

            return View(outcomeList.ToList());
        }

        //
        // GET: /Admin/Outcome/Create
        public ActionResult Create()
        {
			var viewModel = OutcomeViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /Admin/Outcome/Create
        [HttpPost]
        public ActionResult Create(OutcomeViewModel model)
        {
            var outcomeToCreate = new Outcome {Name = model.Name, Description = model.Description};

            var outcomeImage = _fileService.Save(model.File, publicAccess: true);
            outcomeToCreate.ImageUrl = outcomeImage.Uri.AbsoluteUri;

            if (ModelState.IsValid)
            {
                RepositoryFactory.OutcomeRepository.EnsurePersistent(outcomeToCreate);

                Message = "Outcome Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
                var viewModel = OutcomeViewModel.Create(Repository);
                viewModel.Name = model.Name;
                viewModel.Description = model.Description;

                return View(viewModel);
            }
        }

        //
        // GET: /Admin/Outcome/Edit/5
        public ActionResult Edit(Guid id)
        {
            var outcome = RepositoryFactory.OutcomeRepository.GetNullableById(id);

            if (outcome == null) return RedirectToAction("Index");

			var viewModel = OutcomeViewModel.Create(Repository);
            viewModel.Name = outcome.Name;
            viewModel.Description = outcome.Description;
            viewModel.ImageUrl = outcome.ImageUrl;

			return View(viewModel);
        }
        
        //
        // POST: /Admin/Outcome/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, OutcomeViewModel model)
        {
            var outcomeToEdit = RepositoryFactory.OutcomeRepository.GetNullableById(id);

            if (outcomeToEdit == null) return RedirectToAction("Index");

            outcomeToEdit.Name = model.Name;

            if (ModelState.IsValid)
            {
                if (model.File != null) //replace file if we have a new one
                {
                    var badgeImage = _fileService.Save(model.File, publicAccess: true);
                    outcomeToEdit.ImageUrl = badgeImage.Uri.AbsoluteUri;
                }

                RepositoryFactory.OutcomeRepository.EnsurePersistent(outcomeToEdit);

                Message = "Outcome Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
                var viewModel = OutcomeViewModel.Create(Repository);
                viewModel.Name = model.Name;
                viewModel.Name = model.Description;

                return View(viewModel);
            }
        }

        //
        // POST: /Admin/Outcome/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, Outcome outcome)
        {
			var outcomeToDelete = RepositoryFactory.OutcomeRepository.GetNullableById(id);

            if (outcomeToDelete == null) return RedirectToAction("Index");

            RepositoryFactory.OutcomeRepository.Remove(outcomeToDelete);

            Message = "Outcome Removed Successfully";

            return RedirectToAction("Index");
        }
    }

	/// <summary>
    /// ViewModel for the Outcome class
    /// </summary>
    public class OutcomeViewModel
	{
        [Required]
        [StringLength(140)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public HttpPostedFileBase File { get; set; }

        public string ImageUrl { get; set; }
        
		public static OutcomeViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");

		    var viewModel = new OutcomeViewModel();
 
			return viewModel;
		}
	}
}
