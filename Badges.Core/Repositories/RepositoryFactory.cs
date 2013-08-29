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
        IRepositoryWithTypedId<SupportingWork, Guid> SupportingWorkRepository { get; set; }
        IRepositoryWithTypedId<Outcome, Guid> OutcomeRepository { get; set; }
        IRepositoryWithTypedId<Instructor, Guid> InstructorRepository { get; set; }
        IRepositoryWithTypedId<FeedbackRequest, Guid> FeedbackRequestRepository { get; set; }
        void Flush();
    }

    public class RepositoryFactory : IRepositoryFactory
    {
        public IRepositoryWithTypedId<User, Guid> UserRepository { get; set; }
        public IRepositoryWithTypedId<Profile, Guid> ProfileRepository { get; set; }
        public IRepositoryWithTypedId<Role, string> RoleRepository { get; set; }
        public IRepositoryWithTypedId<Experience, Guid> ExperienceRepository { get; set; }
        public IRepositoryWithTypedId<ExperienceType, Guid> ExperienceTypeRepository { get; set; }
        public IRepositoryWithTypedId<Instructor, Guid> InstructorRepository { get; set; }
        public IRepositoryWithTypedId<FeedbackRequest, Guid> FeedbackRequestRepository { get; set; }
        public IRepositoryWithTypedId<Outcome, Guid> OutcomeRepository { get; set; }
        public IRepositoryWithTypedId<SupportingWork, Guid> SupportingWorkRepository { get; set; }

        public void Flush()
        {
            NHibernateSessionManager.Instance.GetSession().Flush();
        }
    }
}
