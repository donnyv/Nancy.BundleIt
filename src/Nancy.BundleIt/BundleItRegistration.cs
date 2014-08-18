using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
