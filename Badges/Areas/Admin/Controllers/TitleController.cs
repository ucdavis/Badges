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
    /// Controller for the Title class
    /// </summary>
    public class TitleController : ApplicationController
    {
        //
        // GET: /Admin/Title/
        public TitleController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            var titleList = RepositoryFactory.TitleRepository.Queryable;

            return View(titleList.ToList());
        }

        //
        // GET: /Admin/Title/Create
        public ActionResult Create()
        {
			var viewModel = TitleViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /Admin/Title/Create
        [HttpPost]
        public ActionResult Create(Title title)
        {
            var titleToCreate = new Title();

            TransferValues(title, titleToCreate);

            if (ModelState.IsValid)
            {
                RepositoryFactory.TitleRepository.EnsurePersistent(titleToCreate);

                Message = "Title Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = TitleViewModel.Create(Repository);
                viewModel.Title = title;

                return View(viewModel);
            }
        }
        
        //
        // POST: /Admin/Title/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
			var titleToDelete = RepositoryFactory.TitleRepository.GetNullableById(id);

            if (titleToDelete == null) return RedirectToAction("Index");

            RepositoryFactory.TitleRepository.Remove(titleToDelete);

            Message = "Title Removed Successfully";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Title source, Title destination)
        {
            destination.Name = source.Name;
        }

    }

	/// <summary>
    /// ViewModel for the Title class
    /// </summary>
    public class TitleViewModel
	{
		public Title Title { get; set; }
 
		public static TitleViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new TitleViewModel {Title = new Title()};
 
			return viewModel;
		}
	}
}
