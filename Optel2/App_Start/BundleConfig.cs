using System.Web;
using System.Web.Optimization;

namespace Optel2
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquerytimepicker").Include(
                       "~/Scripts/jquery.timepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerypaginate").Include(
                      "~/Scripts/jquery.paginate.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.js"));

            bundles.Add(new ScriptBundle("~/bundles/anychart").Include(
                      "~/Scripts/anychart-core.min.js",
                       "~/Scripts/anychart-gantt.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                       "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/jquerytimepicker/css").Include(
                       "~/Content/jquery.timepicker.css"));

            bundles.Add(new StyleBundle("~/bundles/jquerypaginate/css").Include(
                       "~/Content/jquery.paginate.css"));
        }
    }
}
