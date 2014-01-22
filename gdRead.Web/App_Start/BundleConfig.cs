using System.Web.Optimization;

namespace gdRead.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Not really creating bundles just using this for CDN failback
            bundles.UseCdn = true;
            BundleTable.EnableOptimizations = true;
            bundles.Add(new ScriptBundle("~/bundles/jquery", "//ajax.aspnetcdn.com/ajax/jquery/jquery-1.10.2.min.js").Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval", "//ajax.aspnetcdn.com/ajax/jquery.validate/1.11.1/jquery.validate.min.js").Include("~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/angular", "//ajax.googleapis.com/ajax/libs/angularjs/1.2.6/angular.min.js").Include("~/Scripts/angular.1.2.6.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-sanitize", "//ajax.googleapis.com/ajax/libs/angularjs/1.2.6/angular-sanitize.js").Include("~/Scripts/angular-sanitize.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-bootstrap", "//angular-ui.github.io/bootstrap/ui-bootstrap-tpls-0.9.0.js").Include("~/Scripts/angular-bootstrap.js"));
            bundles.Add(new ScriptBundle("~/bundles/myFeeds").Include("~/scripts/myFeeds.js").Include("~/scripts/ngInfiniteScroll.js"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr", "//ajax.aspnetcdn.com/ajax/modernizr/modernizr-2.6.2.js").Include("~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap", "//netdna.bootstrapcdn.com/bootstrap/3.0.3/js/bootstrap.min.js").Include("~/Scripts/bootstrap.js"));
            bundles.Add(new ScriptBundle("~/bundles/respond", "//cdnjs.cloudflare.com/ajax/libs/respond.js/1.4.2/respond.js").Include("~/Scriptns/respond.js"));
            bundles.Add(new StyleBundle("~/Content/bootstrap", "//netdna.bootstrapcdn.com/bootstrap/3.0.3/css/bootstrap.min.css").Include("~/Content/bootstrap.css"));
            bundles.Add(new StyleBundle("~/Content/site").Include("~/Content/site.css"));
        }
    }
}
