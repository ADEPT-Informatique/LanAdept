using System.Web;
using System.Web.Optimization;

namespace LanAdeptAdmin
{
	public class BundleConfig
	{
		// Pour plus d'informations sur le regroupement, visitez http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate*"));

			// Utilisez la version de développement de Modernizr pour le développement et l'apprentissage. Puis, une fois
			// prêt pour la production, utilisez l'outil de génération (bluid) sur http://modernizr.com pour choisir uniquement les tests dont vous avez besoin.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/markdown").Include(
						"~/Scripts/MarkdownDeep.min.js"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/moment.min.js",
					  "~/Scripts/moment-local/fr-ca.js",
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/jasny-bootstrap.js",
					  "~/Scripts/bootstrap-datetimepicker.js",
					  "~/Scripts/respond.js",
					  "~/Scripts/general.js",
					  "~/Scripts/scanner.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap-lan.css",
					  "~/Content/jasny-bootstrap.css",
					  "~/Content/bootstrap-datetimepicker.css",
					  "~/Content/site.css"));
		}
	}
}
