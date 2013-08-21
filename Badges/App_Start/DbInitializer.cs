using Badges.Core;
using FluentNHibernate.Cfg;
using Badges.App_Start;
using NHibernate.Tool.hbm2ddl;

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
            var config = Fluently.Configure().Mappings(m => m.FluentMappings.AddFromAssemblyOf<Profile>());

            config.ExposeConfiguration(c => new SchemaExport(c).Execute(true, true, false)).BuildConfiguration();
        }
    }
}