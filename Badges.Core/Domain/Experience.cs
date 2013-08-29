using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    public class Experience : DomainObjectGuid
    {
        public Experience()
        {
            InstructorViewable = true;
            Created = DateTime.UtcNow;
            Instructors = new List<Instructor>();
            SupportingWorks = new List<SupportingWork>();
            ExperienceOutcomes = new List<ExperienceOutcome>();
        }

        [Required]
        public virtual string Name { get; set; }

        [Required] //Brief description
        [MaxLength(140)]
        public virtual string Description { get; set; }

        public virtual string Organization { get; set; }

        [Required]
        public virtual DateTime Start { get; set; }
        
        public virtual DateTime? End { get; set; }
        
        //TODO: should this be broken into an address?
        public virtual string Location { get; set; }

        public virtual string Notes { get; set; }

        public virtual DateTime Created { get; set; }

        //True if associated instructors can view this experience
        [Display(Name = "Instructor Viewable")]
        public virtual bool InstructorViewable { get; set; }

        [Required]
        public virtual ExperienceType ExperienceType { get; set; }

        [Required]
        public virtual User Creator { get; set; }

        [Required] //TODO: forms do not actually require instructor is selected
        public virtual ICollection<Instructor> Instructors { get; set; }
        public virtual IList<SupportingWork> SupportingWorks { get; set; }
        public virtual IList<ExperienceOutcome> ExperienceOutcomes { get; set; }
        public virtual IList<FeedbackRequest> FeedbackRequests { get; set; }

        public virtual void AddInstructor(Instructor instructor)
        {
            Instructors.Add(instructor);
        }

        public virtual void AddSupportingWork(SupportingWork work)
        {
            work.Experience = this;
            SupportingWorks.Add(work);
        }

        public virtual void AddOutcome(ExperienceOutcome outcome)
        {
            outcome.Experience = this;
            ExperienceOutcomes.Add(outcome);
        }

        public virtual void AddFeedbackRequest(FeedbackRequest feedbackRequest)
        {
            feedbackRequest.Experience = this;
            FeedbackRequests.Add(feedbackRequest);
        }
    }

    public class ExperienceMap : ClassMap<Experience>
    {
        public ExperienceMap()
        {
            Table("Experiences");
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Name).Not.Nullable();
            Map(x => x.Description).Not.Nullable();
            Map(x => x.Start).Not.Nullable();
            Map(x => x.End).Column("`End`");
            Map(x => x.Location);
            Map(x => x.Notes);
            Map(x => x.Created);

            References(x => x.ExperienceType).Not.Nullable();
            References(x => x.Creator).Not.Nullable();

            HasMany(x => x.SupportingWorks).Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.ExperienceOutcomes).Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.FeedbackRequests).Cascade.AllDeleteOrphan().Inverse();
            
            HasManyToMany(x => x.Instructors).AsSet().ParentKeyColumn("Experience_id").ChildKeyColumn("Instructor_id");
        }
    }
}