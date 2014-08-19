using System.Collections.Generic;

using Nancy.BundleIt;

namespace BundleItTestSite
{
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
            

            // ractiveJS
            var YuiRactiveSettings = ConfigSettings.YUICompressionSettings.Js.Clone(settings.GlobalYuiCompressionSettings.Javascript);
            YuiRactiveSettings.ObfuscateJavascript = false;
            YuiRactiveSettings.DisableOptimizations = true;
            var bundleRactive = bundles.AddScripts("ractive", new List<BundleItFile>
            {
                new BundleItFile("app/vendors/ractiveJS/ractive.0.4.0.js", "app/vendors/ractiveJS/ractive.0.4.0.min.js"),
                new BundleItFile("app/vendors/ractiveJS/ractive-transitions-fade.js", YuiRactiveSettings)
            });


            // Base scripts and styles
            var bundleBase = bundles.AddScripts("base", new List<BundleItFile>
            {
                new BundleItFile("app/vendors/jquery.1.11.0.min.js", "//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js", true),
                new BundleItFile(bundleRactive),
                new BundleItFile("app/vendors/lodash.2.4.1.min.js"),
                new BundleItFile("app/common/js/toolbox.js")
            });


            var base_bundle_css = bundles.AddStyles("base", new List<BundleItFile>
            {
                new BundleItFile("app/common/css/common.css")
            });
            
            // Account dashboard
            bundles.AddScripts("accountdashboard", new List<BundleItFile>
            {
                //new BundleItFile(bundleBase),
                //new BundleItFile(bundleRactive),
                new BundleItFile("app/pages/accountdashboard/accountdashboard.js"),
                new BundleItFile("app/pages/accountdashboard/accountdashboardBL.js")
            });

            bundles.AddStyles("accountdashboard", new List<BundleItFile>
            {
                new BundleItFile(base_bundle_css),
                new BundleItFile("app/pages/accountdashboard/accountdashboard.css")
            });


            
            
        }
    }
}
