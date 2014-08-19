# Nancy.BundleIt
## A bundling library made for Nancyfx.

Nancy.BundleIt minifies, obfuscates, concatenates, caches and versions all of your Javascript and CSS files.
It uses the well tested and proven [YUI Compressor for .Net](https://yuicompressor.codeplex.com/) to minify & obfuscates the code.

## Features

 * Minify javascript and css using YUICompressor
 * Override default YUICompressor settings for each file.
 * Bundles files automatically in release mode.
 * Support for CDNs
 * Force release mode or debug mode
 * Force an exception to be thrown when a file is missing.
 * If ".min." is detected in file name, BundleIt will use source as is and not compress it.
 * Works with .NET and Mono.
 * Bundles can be combined to make new bundles. Only need to declare files once!
 * In memory caching for optimal performance.
 * Aggressive HEADER cache settings. Using ETag and Cache-Control. Only download bundle once!

## How to install

Just download nuget package. [![NuGet Version](http://img.shields.io/nuget/v/Nancy.BundleIt.svg?style=flat)](https://www.nuget.org/packages/Nancy.BundleIt/) [![NuGet Downloads](http://img.shields.io/nuget/dt/Nancy.BundleIt.svg?style=flat)](https://www.nuget.org/packages/Nancy.BundleIt/)

Nuget will drop a class into your project called "BundleItConfig.cs".
```csharp
using System.Collections.Generic;
using Nancy.BundleIt;
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
		var bundleRactive = bundles.AddScripts("ractive", new List<BundleItFile>
		{
			new BundleItFile("app/vendors/ractiveJS/ractive.0.4.0.js", "app/vendors/ractiveJS/ractive.0.4.0.min.js"),
			new BundleItFile("app/vendors/ractiveJS/ractive-transitions-fade.js")
		});

		// You can also add a CDN and use bundles you have already created.
		var bundleBase = bundles.AddScripts("base", new List<BundleItFile>
		{
			new BundleItFile("app/vendors/jquery.1.11.0.min.js", "//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js", true),
			new BundleItFile(bundleRactive),
			new BundleItFile("app/vendors/lodash.2.4.1.min.js"),
			new BundleItFile("app/common/js/toolbox.js")
		});

		// Add your CSS also
		var base_bundle_css = bundles.AddStyles("base", new List<BundleItFile>
		{
			new BundleItFile("app/common/css/common.css")
		});
	}
}
```

Remember to add a reference in the "RazorConfig.cs" file.
```csharp
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
```

Then call your bundles in the view.
```
@using Nancy.BundleIt

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <title>Account Dashboard</title>
    @BundleItAssets.RenderCSS("accountdashboard")
	<style type="text/css">

		body {
			text-align: center;
		}

	</style>

</head>
<body>
    <b>Account Dashboard</b>
    @BundleItAssets.RenderJS("accountdashboard")
</body>
</html>
```


