using System;
using System.Linq;
using System.Web.Mvc;
using Badges.Controllers;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using System.Web.Security;

namespace Badges.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the Instructor class
    /// </summary>
    public class InstructorController : ApplicationController
    {
        //
        // GET: /Admin/Instructor/
        public InstructorController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Index()
        {
            var instructorList = RepositoryFactory.InstructorRepository.Queryable;

            return View(instructorList.ToList());
        }
        
        //
        // GET: /Admin/Instructor/Create
        public ActionResult Create()
        {
			var viewModel = InstructorViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /Admin/Instructor/Create
        [HttpPost]
        public ActionResult Create(Instructor instructor)
        {
            if (RepositoryFactory.InstructorRepository.Queryable.Any(i => i.Identifier == instructor.Identifier))
            {
                Message =
                    string.Format(
                        "Instructor could not be created because an instructor with the identifier {0} already exists",
                        instructor.Identifier);
                return RedirectToAction("Index");
            }

            var instructorToCreate = new Instructor();

            TransferValues(instructor, instructorToCreate);

            if (ModelState.IsValid)
            {
                //if the instructor doesn't exist as a user account, create them an account and profile
                var existingUser =
                    RepositoryFactory.UserRepository.Queryable.SingleOrDefault(
                        x => x.Identifier == instructorToCreate.Identifier);

                if (existingUser == null)
                {
                    var profile = new Profile
                        {
                            FirstName = instructorToCreate.FirstName,
                            LastName = instructorToCreate.LastName,
                            Email = instructorToCreate.Identifier
                        };
                    var user = new User {Identifier = instructorToCreate.Identifier};
                    user.AssociateProfile(profile);
                    user.Roles.Add(RepositoryFactory.RoleRepository.GetById(RoleNames.Instructor));
                    RepositoryFactory.UserRepository.EnsurePersistent(user);
                }

                RepositoryFactory.InstructorRepository.EnsurePersistent(instructorToCreate);

                Message = "Instructor Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = InstructorViewModel.Create(Repository);
                viewModel.Instructor = instructor;

                return View(viewModel);
            }
        }

        //
        // GET: /Admin/Instructor/Edit/5
        public ActionResult Edit(Guid id)
        {
            var instructor = RepositoryFactory.InstructorRepository.GetNullableById(id);

            if (instructor == null) return RedirectToAction("Index");

			var viewModel = InstructorViewModel.Create(Repository);
			viewModel.Instructor = instructor;

			return View(viewModel);
        }
        
        //
        // POST: /Admin/Instructor/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, Instructor instructor)
        {
            var instructorToEdit = RepositoryFactory.InstructorRepository.GetNullableById(id);
            
            if (instructorToEdit == null) return RedirectToAction("Index");

            instructor.Identifier = instructorToEdit.Identifier; //Don't allow chaning the identifier
            TransferValues(instructor, instructorToEdit);

            if (ModelState.IsValid)
            {
                RepositoryFactory.InstructorRepository.EnsurePersistent(instructorToEdit);

                Message = "Instructor Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = InstructorViewModel.Create(Repository);
                viewModel.Instructor = instructor;

                return View(viewModel);
            }
        }

        //
        // POST: /Admin/Instructor/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, Instructor instructor)
        {
			var instructorToDelete = RepositoryFactory.InstructorRepository.GetNullableById(id);

            if (instructorToDelete == null) return RedirectToAction("Index");

            RepositoryFactory.InstructorRepository.Remove(instructorToDelete);

            Message = "Instructor Removed Successfully";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Instructor source, Instructor destination)
        {
            destination.FirstName = source.FirstName;
            destination.LastName = source.LastName;
            destination.Identifier = source.Identifier;
            destination.Email = source.Email;
        }
    }

	/// <summary>
    /// ViewModel for the Instructor class
    /// </summary>
    public class InstructorViewModel
	{
		public Instructor Instructor { get; set; }
 
		public static InstructorViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new InstructorViewModel {Instructor = new Instructor()};
 
			return viewModel;
		}
	}
}
