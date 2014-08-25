using Nancy.Helpers;
using Nancy.Responses;

namespace Nancy.BundleIt
{
    public class RequestHandler : NancyModule
    {
        ConfigSettings settings = ConfigSettings.Instance;
        Bundles bundles = Bundles.Instance;

        public RequestHandler()
        {
            Get["/" + settings.ScriptPath + "/{bundlename}.{hash}.js"] = parameters =>
            {
                var bundle = bundles.GetBundle((string)parameters.bundlename, Bundles.eBundleType.script);
                return CacheHelpers.ReturnNotModified(bundle.ETag, null, this.Context)
                    ? ResponseNotModified()
                    : ResponseWithBundle(bundle, "application/javascript");
            };

            Get["/" + settings.StylePath + "/{bundlename}.{hash}.css"] = parameters =>
            {
                var bundle = bundles.GetBundle((string)parameters.bundlename, Bundles.eBundleType.style);
                return CacheHelpers.ReturnNotModified(bundle.ETag, null, this.Context)
                    ? ResponseNotModified()
                    : ResponseWithBundle(bundle, "text/css");
            };
        }

        static Response ResponseNotModified()
        {
            return new Response
            {
                StatusCode = HttpStatusCode.NotModified,
                ContentType = null,
                Contents = Nancy.Response.NoBody
            };
        }

        Response ResponseWithBundle(BundleAsset bundle, string contentType)
        {
            var response = new TextResponse(bundle.obfuscated_source, contentType);
            response.Headers["ETag"] = bundle.ETag;
            response.Headers["Cache-Control"] = bundle.GetCacheControl();
            return response;
        }
    }
}