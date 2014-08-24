using System.Collections.Generic;

using Nancy.BundleIt;

namespace $rootnamespace$
{
	/// <summary>
    /// Configures Nancy.BundleIt assets on application startup.
	/// There can only be 1 BundleItConfig.cs file per project. BundleIt will use the first one it finds.
    /// </summary>
    public class BundleItConfig : IBundleItConfig
    {
        public void Configure(Bundles bundles, ConfigSettings settings)
        {
            // Optional settings
            //settings.ScriptPath = "_scriptbundle";
            //settings.StylePath = "_stylebundle";
            //settings.ThrowExceptionWhenFileMissing = true;
            //settings.ForceDebugMode = false;
            //settings.ForceReleaseMode = true;
            

            // Add scripts based on relative path from root project.
            //var bundleRactive = bundles.AddScripts("ractive", new List<BundleItFile>
            //{
            //    new BundleItFile("app/vendors/ractiveJS/ractive.0.4.0.js", "app/vendors/ractiveJS/ractive.0.4.0.min.js"),
            //    new BundleItFile("app/vendors/ractiveJS/ractive-transitions-fade.js")
            //});

            //// You can also add a CDN and use bundles you have already created.
            //var bundleBase = bundles.AddScripts("base", new List<BundleItFile>
            //{
            //    new BundleItFile("app/vendors/jquery.1.11.0.min.js", "//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js", true),
            //    new BundleItFile(bundleRactive),
            //    new BundleItFile("app/vendors/lodash.2.4.1.min.js"),
            //    new BundleItFile("app/common/js/toolbox.js")
            //});

            //// Add your CSS also
            //var base_bundle_css = bundles.AddStyles("base", new List<BundleItFile>
            //{
            //    new BundleItFile("app/common/css/common.css")
            //});
        }
    }
}
