using Nancy.Bootstrapper;

namespace Nancy.BundleIt
{
    public class BundleItRegistration : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            BundleItAssets.BuildIt();
        }
    }
}
