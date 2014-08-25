using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nancy.BundleIt
{
    [Serializable]
    public class BundleItFile
    {
        public BundleItFile(string relativepath)
        {
            this.debugrelativepath = relativepath;
        }
        public BundleItFile(string relativepath, ConfigSettings.YUICompressionSettings.Js YuiJsSettings) : this(relativepath, string.Empty, false, YuiJsSettings) { }
        public BundleItFile(string relativepath, ConfigSettings.YUICompressionSettings.CSS YuiCssSettings) : this(relativepath, string.Empty, false, YuiCssSettings) { }
        public BundleItFile(string debugrelativepath, string minifiedrelativepath)
        {
            this.debugrelativepath = debugrelativepath;
            this.minifiedrelativepath = minifiedrelativepath;
        }
        public BundleItFile(string debugrelativepath, string minifiedrelativepath, bool IsCDN)
        {
            this.debugrelativepath = debugrelativepath;
            this.minifiedrelativepath = minifiedrelativepath;
            this.IsCDN = IsCDN;
        }
        public BundleItFile(string debugrelativepath, string minifiedrelativepath, bool IsCDN, ConfigSettings.YUICompressionSettings.Js YuiJsSettings)
        {
            this.debugrelativepath = debugrelativepath;
            this.minifiedrelativepath = minifiedrelativepath;
            this.IsCDN = IsCDN;
            this.YuiJsSettings = YuiJsSettings;
        }
        public BundleItFile(string debugrelativepath, string minifiedrelativepath, bool IsCDN, ConfigSettings.YUICompressionSettings.CSS YuiCssSettings)
        {
            this.debugrelativepath = debugrelativepath;
            this.minifiedrelativepath = minifiedrelativepath;
            this.IsCDN = IsCDN;
            this.YuiCssSettings = YuiCssSettings;
        }
        public BundleItFile(Bundle bundle)
        {
            this.bundle_ref_name = bundle.Name.ToUpper();
        }

        public string debugrelativepath { get; set; }
        public string minifiedrelativepath { get; set; }
        public bool IsCDN { get; set; }

        internal string normalizedpath { get { return string.IsNullOrEmpty(this.debugrelativepath) ? string.Empty : this.debugrelativepath.ToUpper(); } }
        internal string bundle_name { get; set; }
        internal string bundle_ref_name { get; set; }
        internal int? sub_bundle_num { get; set; }
        internal string resolved_bundle_name
        {
            get
            {
                return sub_bundle_num == null ? this.bundle_name : this.bundle_name + "_" + this.sub_bundle_num.ToString();
            }
        }
        internal bool has_minified
        {
            get
            {
                if (string.IsNullOrEmpty(this.minifiedrelativepath))
                    return this.debugrelativepath.ToLower().IndexOf(".min.") == -1 ? false : true;
                else
                    return true;
            }
        }
        
        internal ConfigSettings.YUICompressionSettings.Js YuiJsSettings { get; set; }
        internal ConfigSettings.YUICompressionSettings.CSS YuiCssSettings { get; set; }

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
