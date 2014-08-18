using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nancy.ViewEngines.Razor;

public class RazorConfig : IRazorConfiguration
{
    public IEnumerable<string> GetAssemblyNames()
    {
        yield return "Nancy.BundleIt";
    }

    public IEnumerable<string> GetDefaultNamespaces()
    {
        yield return "Nancy.BundleIt";
    }

    public bool AutoIncludeModelNamespace
    {
        get { return false; }
    }
}
