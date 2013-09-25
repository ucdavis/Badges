using System;
using System.Linq;
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
        // GET: /Admin/Outcome/Details/5
        public ActionResult Details(Guid id)
        {
            var outcome = RepositoryFactory.OutcomeRepository.GetNullableById(id);

            if (outcome == null) return RedirectToAction("Index");

            return View(outcome);
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
        public ActionResult Create(Outcome outcome)
        {
            var outcomeToCreate = new Outcome();

            TransferValues(outcome, outcomeToCreate);

            if (ModelState.IsValid)
            {
                RepositoryFactory.OutcomeRepository.EnsurePersistent(outcomeToCreate);

                Message = "Outcome Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = OutcomeViewModel.Create(Repository);
                viewModel.Outcome = outcome;

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
			viewModel.Outcome = outcome;

			return View(viewModel);
        }
        
        //
        // POST: /Admin/Outcome/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, Outcome outcome)
        {
            var outcomeToEdit = RepositoryFactory.OutcomeRepository.GetNullableById(id);

            if (outcomeToEdit == null) return RedirectToAction("Index");

            TransferValues(outcome, outcomeToEdit);

            if (ModelState.IsValid)
            {
                RepositoryFactory.OutcomeRepository.EnsurePersistent(outcomeToEdit);

                Message = "Outcome Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = OutcomeViewModel.Create(Repository);
                viewModel.Outcome = outcome;

                return View(viewModel);
            }
        }
        
        //
        // GET: /Admin/Outcome/Delete/5 
        public ActionResult Delete(Guid id)
        {
			var outcome = RepositoryFactory.OutcomeRepository.GetNullableById(id);

            if (outcome == null) return RedirectToAction("Index");

            return View(outcome);
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
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Outcome source, Outcome destination)
        {
			//Recommendation: Use AutoMapper
			//Mapper.Map(source, destination)
            throw new NotImplementedException();
        }

    }

	/// <summary>
    /// ViewModel for the Outcome class
    /// </summary>
    public class OutcomeViewModel
	{
		public Outcome Outcome { get; set; }
 
		public static OutcomeViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new OutcomeViewModel {Outcome = new Outcome()};
 
			return viewModel;
		}
	}
}
