using System;
using System.Reflection;

using Nancy.ViewEngines.Razor;

namespace Nancy.BundleIt
{
    public class BundleItAssets
    {
        static Bundles _bundles = null;
        static ConfigSettings _settings = null;

        public static IHtmlString RenderJS(string bundlename)
        {
            var bundles = Bundles.Instance;
            return new NonEncodedHtmlString(bundles.GetBundleTags(bundlename, Bundles.eBundleType.script));
        }

        public static IHtmlString RenderCSS(string bundlename)
        {
            var bundles = Bundles.Instance;
            return new NonEncodedHtmlString(bundles.GetBundleTags(bundlename, Bundles.eBundleType.style));
        }

        public static void BuildIt()
        {
            GetConfig();
        }

        static void GetConfig()
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.IsDynamic)
                    continue;


                foreach (Type t in a.GetTypes())
                {
                    if (t.GetInterface(typeof(IBundleItConfig).Name) != null)
                    {
                        _settings = ConfigSettings.Instance;
                        _bundles = Bundles.Instance;
                        
                        dynamic obj = Activator.CreateInstance(t);
                        obj.Configure(_bundles, _settings);

                        _bundles.BuildBundles();

                        return;
                    }
                }
            }
        }
    }
}
