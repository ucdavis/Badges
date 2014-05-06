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
        void Notify(User user, string title, string message);
        void NotifyAdministrators(string title, string message);
    }

    public class NotificationService : INotificationService
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public NotificationService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public void Notify(User user, string title, string message)
        {
            _repositoryFactory.NotificationRepository.EnsurePersistent(new Notification
                {
                    Created = DateTime.UtcNow,
                    Pending = true,
                    To = user,
                    Title = title,
                    Message = message
                });
        }

        public void NotifyAdministrators(string title, string message)
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
                    Title = title,
                    Message = message
                });
            }
            
        }
    }
}