using System.Web.Optimization;
using Badges.App_Start;

[assembly: WebActivator.PostApplicationStartMethod(typeof(BundleBootstrapper), "PostStart")]
namespace Badges.App_Start
{
    /// <summary>
    /// Configures additional bundles, like bootstrap and datatables
    /// </summary>
    public class BundleBootstrapper
    {
        private static void PostStart()
        {
            const string dataTablesVersion = "1.9.4";

            var bundles = BundleTable.Bundles;

            bundles.Add(new ScriptBundle("~/bundles/datatables")
                            .Include(string.Format("~/Scripts/DataTables-{0}/media/js/jquery.dataTables.js", dataTablesVersion))
                            .Include("~/Scripts/datatables-bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout")
                .Include("~/Scripts/knockout-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/typeahead")
                            .Include("~/Scripts/typeahead.js"));

            bundles.Add(new ScriptBundle("~/bundles/chosen")
                            .Include("~/Scripts/Chosen/chosen.jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/datepicker")
                            .Include("~/Scripts/bootstrap-datepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/isotope")
                            .Include("~/Scripts/jquery.isotope.js"));

            // Note: Including bootstrap responsive-- comment it out if you don't need responsive css
            //Also using custom theme build of bootstrap (bootstrap-custom).  Replace with bootstrap.css for default
            //bundles.Add(new StyleBundle("~/Content/styles")
            //                .Include("~/Content/bootstrap-united.css")
            //                .Include("~/Content/font-awesome.css")
            //                .Include("~/Content/chosen.css")
            //                .Include("~/Content/custom.css"));

            bundles.Add(new StyleBundle("~/Content/styles")
                            .Include("~/Content/bootstrap/bootstrap.css")
                            .Include("~/Content/font-awesome.css")
                            .Include("~/Content/chosen.css")
                            .Include("~/Content/badges.css")
                            .Include("~/Content/animate.css"));

            bundles.Add(new StyleBundle("~/Content/static")
                            .Include("~/Content/bootstrap/bootstrap.css")
                            .Include("~/Content/badges.css")
                            .Include("~/Content/static.css"));

            bundles.Add(new StyleBundle("~/Content/typeahead").Include("~/Content/typeahead-bootstrap.css"));
            
            bundles.Add(new StyleBundle("~/Content/datepicker").Include("~/Content/bootstrap-datepicker.css"));

            // Note: Including datatables helper css for bootstrap (http://datatables.net/blog/Twitter_Bootstrap_2)
            bundles.Add(new StyleBundle(string.Format("~/Content/DataTables-{0}/media/css/dataTables", dataTablesVersion))
                            .Include(string.Format("~/Content/DataTables-{0}/media/css/jquery.dataTables.css", dataTablesVersion))
                            .Include(string.Format("~/Content/DataTables-{0}/media/css/datatables-bootstrap.css", dataTablesVersion)));

            ConfigurePageBundles(bundles);
        }

        private static void ConfigurePageBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/studentexperience").Include("~/Scripts/public/studentexperience.js"));
            bundles.Add(new ScriptBundle("~/bundles/modifyexperience").Include("~/Scripts/public/modifyexperience.js"));
            bundles.Add(new ScriptBundle("~/bundles/badgecreate").Include("~/Scripts/public/badgecreate.js"));
            bundles.Add(new ScriptBundle("~/bundles/earn").Include("~/Scripts/public/earn.js"));
        }
    }
}