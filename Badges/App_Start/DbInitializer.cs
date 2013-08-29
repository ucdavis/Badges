using Badges.Core.Domain;
using FluentNHibernate.Cfg;
using Badges.App_Start;
using NHibernate.Tool.hbm2ddl;
using System;

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
            //return;
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
                    var studentRole = new Role("S") {Name = "Student"};
                    session.SaveOrUpdate(studentRole);
                    session.SaveOrUpdate(new Role("I") {Name = "Instructor"});

                    var user = new User {Identifier = "postit"};
                    user.Profile = new Profile(user) {FirstName = "Scott", LastName = "Kirkland", Email = "srkirkland@ucdavis.edu"};
                    user.Roles.Add(studentRole);

                    session.SaveOrUpdate(user);

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
    }
}