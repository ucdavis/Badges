using Badges.Core.Domain;
using Badges.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Badges.Services
{
    public interface INotificationService
    {
        void Notify(User user, User from, string title, string message, string actionLink);
        void NotifyAdministrators(string title, string message, User from, string actionUrl);
    }

    public class NotificationService : INotificationService
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public NotificationService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public void Notify(User user, User from, string title, string message, string actionLink)
        {
            _repositoryFactory.NotificationRepository.EnsurePersistent(new Notification
                {
                    Created = DateTime.UtcNow,
                    Pending = true,
                    To = user,
                    From = from,
                    Title = title,
                    Message = message,
                    ActionURL = actionLink
                });
        }

        public void NotifyAdministrators(string title, string message, User from, string actionUrl)
        {
            /*
            var adminEmails =
                _repositoryFactory.UserRepository.Queryable.Where(x => x.Roles.Any(r => r.Id == RoleNames.Administrator))
                                  .Select(x => x.Profile.Email).ToList();

            //TODO: generalize to use notification methods
            //TODO: at least configure SMTP in web.config
            using (var smtp = new SmtpClient("smtp.ucdavis.edu"))
            {
                smtp.Send("badges-noreply@ucdavis.edu", string.Join(";", adminEmails), "Badges Admin Notification",
                          message);
            }
             * */
            var administrators = _repositoryFactory.UserRepository.Queryable.Where(x => x.Roles.Any(r => r.Id == RoleNames.Administrator));
            foreach (var admin in administrators)
            {
                _repositoryFactory.NotificationRepository.EnsurePersistent(new Notification
                {
                    Created = DateTime.UtcNow,
                    Pending = true,
                    To = admin,
                    From = from,
                    ActionURL = actionUrl,
                    Title = title,
                    Message = message
                });
            }
            
        }
    }
}