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
                    user.Profile = new Profile(user) {FirstName = "Scott", LastName = "Kirkland"};
                    user.Roles.Add(studentRole);

                    session.SaveOrUpdate(user);

                    var etype = new ExperienceType {Name = "Awesome Experience"};
                    session.SaveOrUpdate(etype);
                    session.SaveOrUpdate(new ExperienceType {Name = "Decent Experience"});

                    var experience = new Experience
                        {
                            Creator = user,
                            ExperienceType = etype,
                            Name = "Sample Experience",
                            Description = "This is a bit of text about exactly what I did in this experience",
                            Start = DateTime.Now,
                            Location = "UC Davis"
                        };

                    session.SaveOrUpdate(experience);

                    tx.Commit();
                }
                   
            }
        }
    }
}