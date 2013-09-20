using Badges.Core.Domain;
using Badges.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Badges.Services
{
    public class NotificationService
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public NotificationService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public void Notify(User user, string message)
        {
            _repositoryFactory.NotificationRepository.EnsurePersistent(new Notification
                {
                    Created = DateTime.UtcNow,
                    Pending = true,
                    To = user,
                    Message = message
                });
        }
    }
}