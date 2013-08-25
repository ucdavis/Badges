using Badges.Core.Domain;
using Badges.Core.Repositories;
using System.Linq;
using System.Web;

namespace Badges.Services
{
    public interface IUserService
    {
        User GetCurrent();
    }

    public class UserService : IUserService
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public UserService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public User GetCurrent()
        {
            return
                _repositoryFactory.UserRepository.Queryable.SingleOrDefault(
                    x => x.Identifier == HttpContext.Current.User.Identity.Name);
        }
    }
}