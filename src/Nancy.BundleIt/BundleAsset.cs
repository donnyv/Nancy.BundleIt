using System;
using System.ComponentModel;
using System.Security.Cryptography;

using Nancy.BundleIt.Extensions;

namespace Nancy.BundleIt
{
    internal class BundleAsset
    {
        public BundleAsset(string obfuscated_source)
        {
            this.obfuscated_source = obfuscated_source;
            this.obfuscated_source_hash = GenerateSHA1Hash(obfuscated_source, true);
            this.ETag = this.obfuscated_source_hash;
            this.CacheControlAccessibility = eCacheControlAccessibilityTypes.Public;
            this.CacheControlLength = 31536000;
        }
        public string obfuscated_source { get; set; }
        public string obfuscated_source_hash { get; set; }
        public string ETag { get; set; }

        public enum eCacheControlAccessibilityTypes
        {
            [Description("public")]
            Public = 0,
            [Description("private")]
            Private = 1
        }
        public eCacheControlAccessibilityTypes CacheControlAccessibility { get; set; }

        /// <summary>
        /// Sets Cache-Control header length.
        /// default: max-age=31536000 which is 1 year.
        /// </summary>
        public int CacheControlLength { get; set; }

        internal string GetCacheControl()
        {
            return this.CacheControlAccessibility.ToDescription() + ", max-age=" + this.CacheControlLength.ToString();
        }

        static string GenerateSHA1Hash(string value, bool removeDashes)
        {
            string hash = string.Empty;
            using (var unmanaged = new SHA1CryptoServiceProvider())
            {
                // Why we use UTF8
                // http://stackoverflow.com/questions/8908336/does-computehash-depend-on-machine-key-when-calculated/8909887#8909887
                // Short anwswer: so that the same hash is produced on any machine its ran on
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(value);

                if (removeDashes)
                    hash = BitConverter.ToString(unmanaged.ComputeHash(buffer, 0, buffer.Length)).Replace("-", "");
                else
                    hash = BitConverter.ToString(unmanaged.ComputeHash(buffer, 0, buffer.Length));
            }
            return hash;
        }
    }
}
