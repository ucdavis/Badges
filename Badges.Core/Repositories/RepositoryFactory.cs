using System;
using Badges.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace Badges.Core.Repositories
{
    public interface IRepositoryFactory
    {
        IRepositoryWithTypedId<User, Guid> UserRepository { get; set; }
        IRepositoryWithTypedId<Profile, Guid> ProfileRepository { get; set; }
        IRepositoryWithTypedId<Role, string> RoleRepository { get; set; }
        IRepositoryWithTypedId<ExperienceType, Guid> ExperienceTypeRepository { get; set; }
        IRepositoryWithTypedId<Experience, Guid> ExperienceRepository { get; set; }
        IRepositoryWithTypedId<SupportingFile, Guid> SupportingFileRepository { get; set; }
        void Flush();
    }

    public class RepositoryFactory : IRepositoryFactory
    {
        public IRepositoryWithTypedId<User, Guid> UserRepository { get; set; }
        public IRepositoryWithTypedId<Profile, Guid> ProfileRepository { get; set; }
        public IRepositoryWithTypedId<Role, string> RoleRepository { get; set; }
        public IRepositoryWithTypedId<Experience, Guid> ExperienceRepository { get; set; }
        public IRepositoryWithTypedId<ExperienceType, Guid> ExperienceTypeRepository { get; set; }
        public IRepositoryWithTypedId<SupportingFile, Guid> SupportingFileRepository { get; set; }

        public void Flush()
        {
            NHibernateSessionManager.Instance.GetSession().Flush();
        }
    }
}
