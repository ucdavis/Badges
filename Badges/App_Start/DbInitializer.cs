using System.Web.Configuration;
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
                    user.Profile = new Profile(user)
                        {
                            FirstName = "Scott",
                            LastName = "Kirkland",
                            Email = "srkirkland@ucdavis.edu",
                            ImageUrl = WebConfigurationManager.AppSettings["DefaultProfilePictureUrl"]
                        };
                    user.Roles.Add(studentRole);

                    session.SaveOrUpdate(user);

                    var hermes = new User {Identifier = "hconrad"};
                    hermes.AssociateProfile(new Profile(hermes)
                        {
                            FirstName = "Hermes",
                            LastName = "Conrad",
                            Email = "hconrad@ucdavis.edu",
                            ImageUrl = WebConfigurationManager.AppSettings["DefaultProfilePictureUrl"]
                        });
                    hermes.Roles.Add(instructorRole);

                    session.SaveOrUpdate(hermes);

                    var farnsworth = new User { Identifier = "hfarnsworth" };
                    farnsworth.AssociateProfile(new Profile(farnsworth)
                    {
                        FirstName = "Hubert",
                        LastName = "Farnsworth",
                        Email = "hubert@planex.com",
                        ImageUrl = WebConfigurationManager.AppSettings["DefaultProfilePictureUrl"]
                    });

                    session.SaveOrUpdate(farnsworth);

                    var instructor = new Instructor
                        {
                            FirstName = "Hubert",
                            LastName = "Farnsworth",
                            Email = "hubert@planex.com",
                            Identifier = "hfarnworth",
                            User = farnsworth
                        };

                    var instructor2 = new Instructor
                    {
                        FirstName = "Hermes",
                        LastName = "Conrad",
                        Email = "hconrad@planex.com",
                        Identifier = "hconrad",
                        User = hermes
                    };

                    session.SaveOrUpdate(instructor);
                    session.SaveOrUpdate(instructor2);

                    var etype = new ExperienceType { Name = "Exploration", Icon = "icon-flag" };
                    session.SaveOrUpdate(etype);
                    session.SaveOrUpdate(new ExperienceType {Name = "Collaboration", Icon = "icon-group"});

                    var outcome = new Outcome {Name = "Outcome 1", Description = "Some outcome", ImageUrl = string.Empty};
                    session.SaveOrUpdate(outcome);
                    session.SaveOrUpdate(new Outcome
                        {
                            Name = "Super Skills",
                            Description = "Pretty good skillz",
                            ImageUrl = string.Empty
                        });

                    var experience = new Experience
                        {
                            Creator = user,
                            ExperienceType = etype,
                            Name = "Sample Experience",
                            Description = "This is a bit of text about exactly what I did in this experience",
                            Start = DateTime.Now,
                            Location = "moab, ut",
                            CoverImageUrl = "https://ucdbadges.blob.core.windows.net/publicimagesdev/e600f3de-a969-45af-b70b-7d2f5285e572"
                        };

                    experience.AddInstructor(instructor);
                    experience.AddInstructor(instructor2);

                    session.SaveOrUpdate(experience);
                    
                    //Badges
                    var category = new BadgeCategory
                        {
                            Name = "SampleCategory",
                            ImageUrl = "https://ucdbadges.blob.core.windows.net/publicimagesdev/12da4d70-207a-45e5-b6e4-f1b418c1802a"
                        };

                    session.SaveOrUpdate(category);

                    var badge = new Badge
                        {
                            Approved = true,
                            Category = category,
                            CreatedOn = DateTime.UtcNow,
                            Name = "First Badge",
                            Description = "Really interesting badge for being awesome",
                            Creator = user,
                            ImageUrl = "https://ucdbadges.blob.core.windows.net/publicimagesdev/12da4d70-207a-45e5-b6e4-f1b418c1802a"
                        };

                    badge.AddCriteria("You need to do a, b, c");
                    badge.AddCriteria("Also you need to be a human");

                    session.SaveOrUpdate(badge);

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