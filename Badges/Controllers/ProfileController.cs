using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using Badges.Models.Profile;
using Badges.Models.Shared;
using ImageResizer;
using SoundInTheory.DynamicImage.Fluent;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;
using Badges.Services;
using Badges.Helpers;

namespace Badges.Controllers
{
    [Authorize]
    public class ProfileController : ApplicationController
    {
        private readonly IFileService _fileService;
        private readonly INotificationService _notificationService;
        private const int ProfilePictureWidth = 300;
        private const int ProfilePictureHeight = 300;

        public ProfileController(IRepositoryFactory repositoryFactory, IFileService fileService, INotificationService notificationService) : base(repositoryFactory)
        {
            _fileService = fileService;
            _notificationService = notificationService;
        }

        public ActionResult Picture()
        {
            return View();
        }

        public ActionResult Crop()
        {
            return View();
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult Crop(HttpPostedFileBase picture, int x1, int x2, int y1, int y2, int w, int h)
        {
            using (var reader = new BinaryReader(picture.InputStream))
            {
                var img = new CompositionBuilder()
                    //.WithLayer(LayerBuilder.Image.SourceFile("~/Content/images/profile-default.jpg")
                    .WithLayer(LayerBuilder.Image.SourceBytes(reader.ReadBytes(picture.ContentLength))
                                           .WithFilter(FilterBuilder.Crop.X(x1).Y(y1).To(w, h)));

                return Redirect(img.Url);
            }
        }

        public ActionResult Create()
        {
            if (RepositoryFactory.UserRepository.Queryable.Any(x => x.Identifier == CurrentUser.Identity.Name))
            {
                Message = "You already have a profile"; //TODO: redirect to existing profile
                return RedirectToAction("Edit");
            }

            var model = new ProfileEditModel
                {
                    Profile = new Profile(),
                    Roles = RepositoryFactory.RoleRepository.Queryable.OrderByDescending(x=>x.Name).ToList()
                };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Profile profile, string roles, HttpPostedFileBase image)
        {
            if (RepositoryFactory.UserRepository.Queryable.Any(x => x.Identifier == CurrentUser.Identity.Name))
            {
                Message = "You already have a profile";
                return RedirectToAction("Edit");
            }

            if (image == null)
            {
                image = new FakeHttpPostedFileBase("default.jpg", "application/jpg",
                                                   System.IO.File.ReadAllBytes(
                                                       Server.MapPath("~/Content/images/profile-default.jpg")));
            }

            profile.ImageUrl = CropAndSave(image, ProfilePictureWidth, ProfilePictureHeight);
            var user = new User {Identifier = CurrentUser.Identity.Name, Profile = profile};
            profile.User = user;

            user.Roles.Add(RepositoryFactory.RoleRepository.GetById(roles));
            RepositoryFactory.UserRepository.EnsurePersistent(user);
            
            return RedirectToAction("Index", "Home");
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
                Roles = RepositoryFactory.RoleRepository.Queryable.OrderByDescending(x=>x.Name).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Profile profile, string roles, HttpPostedFileBase image, bool isInstructor = false)
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
                userProfileToEdit.Profile.ImageUrl = CropAndSave(image, ProfilePictureWidth, ProfilePictureHeight);
            }

            userProfileToEdit.Roles.Clear();
            userProfileToEdit.Roles.Add(RepositoryFactory.RoleRepository.GetById(roles));

            RepositoryFactory.UserRepository.EnsurePersistent(userProfileToEdit);

            if (CurrentUser.IsInRole(RoleNames.Student))
            {
                // See if they requested to be an instructor
                if (isInstructor)
                {
                    // Notify admins
                    _notificationService.NotifyAdministrators("New instructor request",
                        profile.DisplayName + " (" + profile.Email + ") requested Instructor permissions.",
                        AuthenticatedUser,
                        ActionLinkHelper.ActionLink(Url.Action("GrantInstructorPermissions", "User", new { area = string.Empty, name = CurrentUser.Identity.Name}), "Grant instructor permissions"));
                }
            }
            

            Message = "Your profile changes were successful";

            return RedirectToAction("Edit");
        }

        /// <summary>
        /// View either your own profile image, or another user's if you pass their profile id
        /// </summary>
        /// <param name="profileId">profileID, optional</param>
        /// <returns></returns>
        public ActionResult ViewProfileImage(Guid? profileId)
        {
            Profile profile;

            if (profileId.HasValue)
            {
                profile =
                    RepositoryFactory.ProfileRepository.Queryable.SingleOrDefault(x => x.Id == profileId.Value);
            }
            else
            {
                profile =
                    RepositoryFactory.ProfileRepository.Queryable.SingleOrDefault(
                        x => x.User.Identifier == CurrentUser.Identity.Name);
            }

            if (profile == null || string.IsNullOrWhiteSpace(profile.ImageUrl))
            {
                return PartialView(null);
            }

            var model = new ImageModel {Alt = "Profile Image", Width = 120, Height = 120};

            model.Url =
                new CompositionBuilder().WithLayer(
                    LayerBuilder.Image.SourceUrl(profile.ImageUrl)
                                .WithFilter(FilterBuilder.Resize.To(model.Width, model.Height)))
                                        .Url;

            return PartialView(model);
        }

        /// <summary>
        /// Crops the image to the given size, saves, and returns the url of the final blob
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private string CropAndSave(HttpPostedFileBase image, int width, int height)
        {
            var cropResizer = new ResizeSettings(width, height, FitMode.Crop, null);

            using (var stream = new MemoryStream())
            {
                ImageBuilder.Current.Build(image.InputStream, stream, cropResizer, disposeSource: false);
                
                //save the original and modified image
                var blob = _fileService.Save(image.InputStream, stream, image.ContentType, publicAccess: true);

                return blob.Uri.AbsoluteUri;
            }
        }
    }
}
