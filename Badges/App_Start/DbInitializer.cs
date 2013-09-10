using Badges.Core.Domain;
using FluentNHibernate.Cfg;
using Badges.App_Start;
using NHibernate.Tool.hbm2ddl;
using System;
using NHibernate;

[assembly: WebActivator.PreApplicationStartMethod(typeof(DbInitializer), "PreStart")]
namespace Badges.App_Start
{
    public class DbInitializer
    {
        /// <summary>
        /// PreStart for the UCDArch Application configures the model binding, db, and IoC container
        /// </summary>
        public static void PreStart()
        {
            return; //TODO: Comment out if you want to auto-wipe the DB
            ResetDb();
        }

        public static void ResetDb()
        {
            var config =
                Fluently.Configure()
                        .Mappings(
                            m =>
                            m.FluentMappings.AddFromAssemblyOf<Profile>()
                             .Conventions.AddFromAssemblyOf<UCDArch.Data.NHibernate.Fluent.HasManyConvention>());

            config.ExposeConfiguration(c => new SchemaExport(c).Execute(true, true, false)).BuildConfiguration();

            PopulateDb(config);
        }

        private static void PopulateDb(FluentConfiguration config)
        {
            using (var session = config.BuildSessionFactory().OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    PopulateLookups(session);
   
                    var studentRole = new Role(RoleNames.Student) {Name = "Student"};
                    var instructorRole = new Role(RoleNames.Instructor) {Name = "Instructor"};
                    var adminRole = new Role(RoleNames.Administrator) {Name = "Administrator"};
                    session.SaveOrUpdate(studentRole);
                    session.SaveOrUpdate(instructorRole);
                    session.SaveOrUpdate(adminRole);

                    var user = new User {Identifier = "postit"};
                    user.Profile = new Profile(user) {FirstName = "Scott", LastName = "Kirkland", Email = "srkirkland@ucdavis.edu"};
                    user.Roles.Add(studentRole);

                    session.SaveOrUpdate(user);

                    var hermes = new User {Identifier = "hconrad"};
                    hermes.AssociateProfile(new Profile(hermes) { FirstName = "Hermes", LastName = "Conrad", Email = "hconrad@ucdavis.edu" });
                    hermes.Roles.Add(instructorRole);

                    session.SaveOrUpdate(hermes);

                    var instructor = new Instructor
                        {
                            FirstName = "Hubert",
                            LastName = "Farnsworth",
                            Email = "hubert@planex.com",
                            Identifier = "hfarnworth"
                        };

                    var instructor2 = new Instructor
                    {
                        FirstName = "Hermes",
                        LastName = "Conrad",
                        Email = "hconrad@planex.com",
                        Identifier = "hconrad"
                    };

                    session.SaveOrUpdate(instructor);
                    session.SaveOrUpdate(instructor2);

                    var etype = new ExperienceType {Name = "Awesome Experience"};
                    session.SaveOrUpdate(etype);
                    session.SaveOrUpdate(new ExperienceType {Name = "Decent Experience"});

                    var outcome = new Outcome {Name = "Outcome 1"};
                    session.SaveOrUpdate(outcome);
                    session.SaveOrUpdate(new Outcome { Name = "Super Skills"});

                    var experience = new Experience
                        {
                            Creator = user,
                            ExperienceType = etype,
                            Name = "Sample Experience",
                            Description = "This is a bit of text about exactly what I did in this experience",
                            Start = DateTime.Now,
                            Location = "UC Davis"
                        };

                    experience.AddInstructor(instructor);
                    experience.AddInstructor(instructor2);

                    session.SaveOrUpdate(experience);
                    
                    tx.Commit();
                }
                   
            }
        }

        private static void PopulateLookups(ISession session)
        {
            session.SaveOrUpdate(new Title {Name = "MATH 128"});
            session.SaveOrUpdate(new Title {Name = "MATH 127"});
            session.SaveOrUpdate(new Title {Name = "MATH 101"});

            session.SaveOrUpdate(new Organization {Name = "University of California, Davis"});
            session.SaveOrUpdate(new Organization {Name = "Davis Community Gardens"});
        }
    }
}