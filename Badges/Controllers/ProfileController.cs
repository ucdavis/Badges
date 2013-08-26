using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using Badges.Models.Profile;

namespace Badges.Controllers
{
    [Authorize]
    public class ProfileController : ApplicationController
    {
        public ProfileController(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public ActionResult Create()
        {
            if (RepositoryFactory.UserRepository.Queryable.Any(x => x.Identifier == CurrentUser.Identity.Name))
            {
                Message = "You already have a profile"; //TODO: redirect to existing profile
                RedirectToAction("Landing", "Home");
            }

            var model = new ProfileEditModel
                {
                    Profile = new Profile(),
                    Roles = RepositoryFactory.RoleRepository.GetAll()
                };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Profile profile, string roles, HttpPostedFileBase image)
        {
            if (RepositoryFactory.UserRepository.Queryable.Any(x => x.Identifier == CurrentUser.Identity.Name))
            {
                Message = "You already have a profile";
                RedirectToAction("Edit");
            }

            if (image != null)
            {
                profile.ContentType = image.ContentType;

                using (var binaryReader = new BinaryReader(image.InputStream))
                {
                    profile.Image = binaryReader.ReadBytes((int) image.InputStream.Length);
                }
            }

            var user = new User {Identifier = CurrentUser.Identity.Name, Profile = profile};
            profile.User = user;

            user.Roles.Add(RepositoryFactory.RoleRepository.GetById(roles));

            RepositoryFactory.UserRepository.EnsurePersistent(user);

            return RedirectToAction("Landing", "Home");
        }
    
        /// <summary>
        /// Edit your profile
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            var profile =
                RepositoryFactory.ProfileRepository.Queryable.SingleOrDefault(
                    x => x.User.Identifier == CurrentUser.Identity.Name);

            if (profile == null)
            {
                Message = "You don't yet have a profile, please create one now";
                return RedirectToAction("Create");
            }

            var model = new ProfileEditModel
            {
                Profile = profile,
                Roles = RepositoryFactory.RoleRepository.GetAll()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Profile profile, string roles, HttpPostedFileBase image)
        {
            var userProfileToEdit =
                RepositoryFactory.UserRepository.Queryable.SingleOrDefault(
                    x => x.Identifier == CurrentUser.Identity.Name);

            if (userProfileToEdit == null)
            {
                Message = "You don't yet have a profile, please create one now";
                return RedirectToAction("Create");
            }

            UpdateModel(userProfileToEdit.Profile, "Profile", null, new[] {"image"});
            
            if (image != null)
            {
                userProfileToEdit.Profile.ContentType = image.ContentType;

                using (var binaryReader = new BinaryReader(image.InputStream))
                {
                    userProfileToEdit.Profile.Image = binaryReader.ReadBytes((int)image.InputStream.Length);
                }
            }

            userProfileToEdit.Roles.Clear();
            userProfileToEdit.Roles.Add(RepositoryFactory.RoleRepository.GetById(roles));

            RepositoryFactory.UserRepository.EnsurePersistent(userProfileToEdit);

            Message = "Your profile changes were successful";

            return RedirectToAction("Edit");
        }

        public FileResult ViewProfileImage()
        {
            var profile =
                RepositoryFactory.ProfileRepository.Queryable.SingleOrDefault(x => x.User.Identifier == CurrentUser.Identity.Name);

            if (profile == null || profile.Image == null)
            {
                //TODO: Default image?
                return null;
            }

            return File(profile.Image, profile.ContentType);
        }
    }
}
