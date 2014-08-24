using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Nancy.BundleIt.Helpers;

using Yahoo.Yui.Compressor;

namespace Nancy.BundleIt
{
    public sealed class Bundles
    {
        internal enum eBundleType
        {
            script = 0,
            style = 1
        }
        const string _script_tag_template = "<script src='{0}.{1}.js'></script>";
        const string _script_tag_standard_template = "<script src='{0}'></script>";
        const string _style_tag_template = "<link rel='stylesheet' type='text/css' href='{0}.{1}.css'>";
        const string _style_tag_standard_template = "<link rel='stylesheet' type='text/css' href='{0}'>";

        ConfigSettings _settings = ConfigSettings.Instance;
        static readonly Bundles _instance = new Bundles();
        public static Bundles Instance
        {
            get
            {
                return _instance;
            }
        }

        internal List<BundleItFile> _script_files = new List<BundleItFile>();
        internal List<BundleItFile> _style_files = new List<BundleItFile>();
        internal Dictionary<string, string> _script_tag_bundles = new Dictionary<string, string>();
        internal Dictionary<string, string> _style_tag_bundles = new Dictionary<string, string>();
        internal Dictionary<string, BundleAsset> _script_compressed_bundles = new Dictionary<string, BundleAsset>();
        internal Dictionary<string, BundleAsset> _style_compressed_bundles = new Dictionary<string, BundleAsset>();
        
        public Bundle AddScripts(string bundlename, List<BundleItFile> files)
        {
            bundlename = bundlename.ToUpper();
            files.ForEach(f => f.bundle_name = bundlename);

            var clone = new BundleItFile[files.Count];
            files.CopyTo(clone);
            _script_files.AddRange(clone);

            return new Bundle(bundlename);
        }

        public Bundle AddStyles(string bundlename, List<BundleItFile> files)
        {
            bundlename = bundlename.ToUpper();
            files.ForEach(f => f.bundle_name = bundlename);

            var clone = new BundleItFile[files.Count];
            files.CopyTo(clone);
            _style_files.AddRange(clone);

            return new Bundle(bundlename);
        }

        internal string GetBundleTags(string name, eBundleType type)
        {
            // normalize
            var n = name.ToUpper();

            if (type == eBundleType.script)
            {
                if (!_script_tag_bundles.ContainsKey(n))
                    throw new Exception("Script bundle name '" + name + "' could not be found.");

                return _script_tag_bundles[n];
            }

            if (type == eBundleType.style)
            {
                if (!_style_tag_bundles.ContainsKey(n))
                    throw new Exception("Style bundle name '" + name + "' could not be found.");

                return _style_tag_bundles[n];
            }
            
            return string.Empty;
        }

        internal BundleAsset GetBundle(string name, eBundleType type)
        {
            // normalize
            var n = name.ToUpper();

            if (type == eBundleType.script)
            {
                if (!_script_compressed_bundles.ContainsKey(n))
                    throw new Exception("Script bundle name '" + name + "' could not be found.");

                return _script_compressed_bundles[n];
            }

            if (type == eBundleType.style)
            {
                if (!_style_compressed_bundles.ContainsKey(n))
                    throw new Exception("Style bundle name '" + name + "' could not be found.");

                return _style_compressed_bundles[n];
            }

            return null;
        }

        internal void BuildBundles()
        {
            if (_settings.IsDebugMode())
                BuildDebugBundles();
            else
                BuildReleaseBundles();
        }

        void BuildDebugBundles()
        {
            StringBuilder bundlestring = null;

            // scripts
            var script_bundle_names = _script_files.DistinctBy(sf => sf.bundle_name).Select(sf => sf.bundle_name).ToList();
            foreach (var bname in script_bundle_names)
            {
                var bundlefiles = _script_files.Where(f => f.bundle_name == bname).ToList();
                bundlestring = new StringBuilder();
                LoadDebugBundles(bname, bundlefiles, bundlestring, eBundleType.script);
                _script_tag_bundles.Add(bname, bundlestring.ToString());
            }

            // styles
            var style_bundle_names = _style_files.DistinctBy(sf => sf.bundle_name).Select(sf => sf.bundle_name).ToList();
            foreach (var bname in style_bundle_names)
            {
                var bundlefiles = _style_files.Where(f => f.bundle_name == bname).ToList();
                bundlestring = new StringBuilder();
                LoadDebugBundles(bname, bundlefiles, bundlestring, eBundleType.style);
                _style_tag_bundles.Add(bname, bundlestring.ToString());
            }

        }

        void LoadDebugBundles(string bundlename, List<BundleItFile> files, StringBuilder bundlestring, eBundleType type)
        {
            foreach (var s in files)
            {
                // load referenced bundle files
                if(!string.IsNullOrEmpty(s.bundle_ref_name)){

                    // get files by bundle name
                    List<BundleItFile> refbundlefiles = null;
                    if (type == eBundleType.script)
                        refbundlefiles = _script_files.Where(f => f.bundle_name == s.bundle_ref_name).ToList();

                    if (type == eBundleType.style)
                        refbundlefiles = _style_files.Where(f => f.bundle_name == s.bundle_ref_name).ToList();

                    LoadDebugBundles(s.bundle_ref_name, refbundlefiles, bundlestring, type);
                }
                else
                {
                    if(type == eBundleType.script)
                        bundlestring.Append(string.Format(_script_tag_standard_template, s.debugrelativepath) + "\n");
                    
                    if(type == eBundleType.style)
                        bundlestring.Append(string.Format(_style_tag_standard_template, s.debugrelativepath) + "\n");

                }
                    
            }
        }

        void BuildReleaseBundles()
        {
            List<BundleItFile> BundleResolvedFiles = null;

            // scripts
            var script_bundle_names = _script_files.DistinctBy(sf => sf.bundle_name).Select(sf => sf.bundle_name).ToList();
            foreach (var bname in script_bundle_names)
            {
                var bundlefiles = _script_files.Where(f => f.bundle_name == bname).ToList();
                BundleResolvedFiles = new List<BundleItFile>();
                LoadReleaseBundles(bname, bundlefiles, BundleResolvedFiles, eBundleType.script);

                // check for cdn's
                SplitBundlesWithCDNs(BundleResolvedFiles);
                
                // generate release bundles
                GenerateReleaseBundles(BundleResolvedFiles, eBundleType.script);
                
                // generate bundle tags
                GenerateReleaseBundleTags(BundleResolvedFiles, eBundleType.script);
            }

            // styles
            var style_bundle_names = _style_files.DistinctBy(sf => sf.bundle_name).Select(sf => sf.bundle_name).ToList();
            foreach (var bname in style_bundle_names)
            {
                var bundlefiles = _style_files.Where(f => f.bundle_name == bname).ToList();
                BundleResolvedFiles = new List<BundleItFile>();
                LoadReleaseBundles(bname, bundlefiles, BundleResolvedFiles, eBundleType.style);

                // check for cdn's
                SplitBundlesWithCDNs(BundleResolvedFiles);

                // generate release bundles
                GenerateReleaseBundles(BundleResolvedFiles, eBundleType.style);

                // generate bundle tags
                GenerateReleaseBundleTags(BundleResolvedFiles, eBundleType.style);
            }
            
        }

        void LoadReleaseBundles(string bundlename, List<BundleItFile> files, List<BundleItFile> bundle_resolved_files, eBundleType type)
        {
            foreach (var s in files)
            {
                // load referenced bundle files
                if (!string.IsNullOrEmpty(s.bundle_ref_name))
                {

                    // get files by bundle name
                    List<BundleItFile> refbundlefiles = null;
                    if (type == eBundleType.script)
                        refbundlefiles = _script_files.Where(f => f.bundle_name == s.bundle_ref_name).ToList();

                    if (type == eBundleType.style)
                        refbundlefiles = _style_files.Where(f => f.bundle_name == s.bundle_ref_name).ToList();

                    LoadReleaseBundles(bundlename, refbundlefiles, bundle_resolved_files, type);
                }
                else
                {
                    var clone = BundleItFile.Clone(s);
                    clone.bundle_name = bundlename;
                    bundle_resolved_files.Add(clone);
                }

            }
        }

        void SplitBundlesWithCDNs(List<BundleItFile> BundleResolvedFiles)
        {
            if (!BundleResolvedFiles.Exists(f => f.IsCDN))
                return;

            int i = 1;
            int prevsub = 0;
            foreach (var f in BundleResolvedFiles)
            {
                if (f.IsCDN)
                {
                    i = prevsub == i ? 1 + i : i;
                    f.sub_bundle_num = i;
                    ++i;
                }
                else
                {
                    f.sub_bundle_num = i;
                    prevsub = i;
                }
                      
            }
        }

        void GenerateReleaseBundles(List<BundleItFile> BundleResolvedFiles, eBundleType type)
        {
            var RootFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            var unique_resolved_bundles = BundleResolvedFiles.DistinctBy(b => b.resolved_bundle_name);

            foreach (var rbundle in unique_resolved_bundles)
            {
                if (rbundle.IsCDN)
                    continue;

                var compressed_bundle = new StringBuilder();
                var bfiles = BundleResolvedFiles.Where(bsf => bsf.resolved_bundle_name == rbundle.resolved_bundle_name);
                foreach (var f in bfiles)
                {
                    // if has_minified returns true, then find which path to use.
                    var path = string.Empty;
                    if (f.has_minified)
                        path = string.IsNullOrEmpty(f.minifiedrelativepath) ? f.debugrelativepath : f.minifiedrelativepath;
                    else
                        path = f.debugrelativepath;


                    // check for leading '/'
                    if (path[0] == "/".ToCharArray()[0])
                        path = path.Substring(1);


                    var filesource = LoadFile(Path.Combine(RootFolder, path));

                    // run it through YUI if not already minified
                    string compressed_src = string.Empty;
                    if (f.has_minified)
                        compressed_src = filesource;
                    else
                    {
                        if (type == eBundleType.script)
                        {
                            var YuiJsSettings = f.YuiJsSettings == null ? ConfigSettings.Instance.GlobalYuiCompressionSettings.Javascript : f.YuiJsSettings;
                            compressed_src = YUI_JS(filesource, YuiJsSettings);
                        }
                        if (type == eBundleType.style)
                        {
                            var YuiCssSettings = f.YuiCssSettings == null ? ConfigSettings.Instance.GlobalYuiCompressionSettings.Css : f.YuiCssSettings;
                            compressed_src = YUI_CSS(filesource, YuiCssSettings);
                        }
                    }

                    compressed_bundle.Append(compressed_src);
                }

                if (type == eBundleType.script)
                    _script_compressed_bundles.Add(rbundle.resolved_bundle_name, new BundleAsset(compressed_bundle.ToString()));

                if (type == eBundleType.style)
                    _style_compressed_bundles.Add(rbundle.resolved_bundle_name, new BundleAsset(compressed_bundle.ToString()));
            }
        }

        void GenerateReleaseBundleTags(List<BundleItFile> BundleResolvedFiles, eBundleType type)
        {
            var bname = string.Empty;
            var path = type == eBundleType.script ? _settings.ScriptPath : _settings.StylePath;
            var tag_template = type == eBundleType.script ? _script_tag_template : _style_tag_template;
            var tag_cdn_template = type == eBundleType.script ? _script_tag_standard_template : _style_tag_standard_template;
            var obfuscated_source_hash = string.Empty;

            StringBuilder tags;
            var unique_bundles = BundleResolvedFiles.DistinctBy(b => b.bundle_name);
            foreach (var bundle in unique_bundles)
            {
                tags = new StringBuilder();
                var unique_resolved_bundles = BundleResolvedFiles.Where(b => b.bundle_name == bundle.bundle_name).DistinctBy(b2 => b2.resolved_bundle_name);
                
                foreach (var rbundle in unique_resolved_bundles)
                {
                    if (rbundle.IsCDN)
                        tags.AppendLine(string.Format(tag_cdn_template, rbundle.minifiedrelativepath));
                    else
                    {
                        if (type == eBundleType.script)
                            obfuscated_source_hash = _script_compressed_bundles[rbundle.resolved_bundle_name].obfuscated_source_hash;

                        if (type == eBundleType.style)
                            obfuscated_source_hash = _style_compressed_bundles[rbundle.resolved_bundle_name].obfuscated_source_hash;


                        tags.AppendLine(string.Format(tag_template, "/" + path + "/" + rbundle.resolved_bundle_name.ToLower(), obfuscated_source_hash));
                    }
                }

                if (type == eBundleType.script)
                    _script_tag_bundles.Add(bundle.bundle_name, tags.ToString());

                if (type == eBundleType.style)
                    _style_tag_bundles.Add(bundle.bundle_name, tags.ToString());
            }
        }

        string LoadFile(string filepath)
        {

            if(!File.Exists(filepath)){

                if(ConfigSettings.Instance.ThrowExceptionWhenFileMissing)
                    throw new FileNotFoundException("Could not find file '" + filepath + "'.", filepath);
                else
                    return string.Empty;
            }

            var lines = File.ReadLines(filepath);
            return string.Join(Environment.NewLine, lines.ToArray());
        }

        string YUI_JS(string filesource, ConfigSettings.YUICompressionSettings.Js JsSettings)
        {
            try
            {
                var jscompressor = new JavaScriptCompressor();
                jscompressor.CompressionType = JsSettings.CompressionType;
                jscompressor.DisableOptimizations = JsSettings.DisableOptimizations;
                jscompressor.Encoding = JsSettings.Encoding;
                jscompressor.IgnoreEval = JsSettings.IgnoreEval;
                jscompressor.LineBreakPosition = JsSettings.LineBreakPosition;
                jscompressor.ObfuscateJavascript = JsSettings.ObfuscateJavascript;
                jscompressor.PreserveAllSemicolons = JsSettings.PreserveAllSemicolons;
                jscompressor.ThreadCulture = JsSettings.ThreadCulture;

                return jscompressor.Compress(filesource);
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
                return filesource;
            }
        }

        string YUI_CSS(string filesource, ConfigSettings.YUICompressionSettings.CSS CssSettings)
        {
            try
            {
                var csscompressor = new CssCompressor();
                csscompressor.CompressionType = CssSettings.CompressionType;
                csscompressor.LineBreakPosition = CssSettings.LineBreakPosition;
                csscompressor.RemoveComments = CssSettings.RemoveComments;

                return csscompressor.Compress(filesource);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return filesource;
            }
        }

        
    }
}
