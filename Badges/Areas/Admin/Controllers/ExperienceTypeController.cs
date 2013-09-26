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

namespace Badges.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the ExperienceType class
    /// </summary>
    [Authorize(Roles = RoleNames.Administrator)]
    public class ExperienceTypeController : ApplicationController
    {
        //
        // GET: /Admin/ExperienceType/
        public ExperienceTypeController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            var experienceTypes = RepositoryFactory.ExperienceTypeRepository.Queryable;

            return View(experienceTypes.ToList());
        }

        //
        // GET: /Admin/ExperienceType/Create
        public ActionResult Create()
        {
			var viewModel = ExperienceTypeViewModel.Create(Repository);

            return View(viewModel);
        }

        //
        // POST: /Admin/ExperienceType/Create
        [HttpPost]
        public ActionResult Create(ExperienceTypeViewModel model)
        {
            var experienceTypeToCreate = new ExperienceType();
            TransferValues(model, experienceTypeToCreate);

            if (ModelState.IsValid)
            {
                RepositoryFactory.ExperienceTypeRepository.EnsurePersistent(experienceTypeToCreate);

                Message = "ExperienceType Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				return View(model);
            }
        }

        //
        // GET: /Admin/ExperienceType/Edit/5
        public ActionResult Edit(Guid id)
        {
            var experienceType = RepositoryFactory.ExperienceTypeRepository.GetNullableById(id);

            if (experienceType == null) return RedirectToAction("Index");

			var viewModel = ExperienceTypeViewModel.Create(Repository);
            viewModel.Name = experienceType.Name;
            viewModel.Icon = experienceType.Icon;

			return View(viewModel);
        }
        
        //
        // POST: /Admin/ExperienceType/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, ExperienceTypeViewModel model)
        {
            var experienceTypeToEdit = RepositoryFactory.ExperienceTypeRepository.GetNullableById(id);

            if (experienceTypeToEdit == null) return RedirectToAction("Index");

            TransferValues(model, experienceTypeToEdit);
            
            if (ModelState.IsValid)
            {
                RepositoryFactory.ExperienceTypeRepository.EnsurePersistent(experienceTypeToEdit);

                Message = "ExperienceType Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }
        
        //
        // POST: /Admin/ExperienceType/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, ExperienceType ExperienceType)
        {
			var experienceTypeToDelete = RepositoryFactory.ExperienceTypeRepository.GetNullableById(id);
            
            if (experienceTypeToDelete == null) return RedirectToAction("Index");

            RepositoryFactory.ExperienceTypeRepository.Remove(experienceTypeToDelete);

            Message = "ExperienceType Removed Successfully";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(ExperienceTypeViewModel source, ExperienceType destination)
        {
            destination.Name = source.Name;
            destination.Icon = source.Icon;
        }

    }

	/// <summary>
    /// ViewModel for the ExperienceType class
    /// </summary>
    public class ExperienceTypeViewModel
	{
        [Required]
        [StringLength(140)]
        public string Name { get; set; }

        [Required]
        [StringLength(140)]
	    public string Icon { get; set; }

        public static ExperienceTypeViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new ExperienceTypeViewModel();
 
			return viewModel;
		}
	}
}
