using System;
using System.Text;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Yahoo.Yui.Compressor;

namespace Nancy.BundleIt
{
    public sealed class ConfigSettings
    {
        static readonly ConfigSettings _instance = new ConfigSettings();

        public ConfigSettings()
        {
            this.GlobalYuiCompressionSettings = new YUICompressionSettings();
        }
        public static ConfigSettings Instance
        {
            get
            {
                return _instance;
            }
        }

        const string _scriptPathDefault = "_script";
        const string _stylePathDefault = "_style";

        string _scriptPath = string.Empty;
        public string ScriptPath
        {
            get
            {
                return string.IsNullOrEmpty(_scriptPath) ? _scriptPathDefault : _scriptPath;
            }
            set
            {
                _scriptPath = value;
            }
        }

        string _stylePath = string.Empty;
        public string StylePath
        {
            get
            {
                return string.IsNullOrEmpty(_stylePath) ? _stylePathDefault : _stylePath;
            }
            set
            {
                _stylePath = value;

                
                var jcompress = new JavaScriptCompressor();
            
            }
        }

        public bool ForceDebugMode { get; set; }

        public bool ForceReleaseMode { get; set; }

        public bool ThrowExceptionWhenFileMissing { get; set; }

        public class YUICompressionSettings
        {
            public YUICompressionSettings()
            {
                this.Javascript = new Js();
                this.Css = new CSS();
            }

            public Js Javascript { get; set; }
            public CSS Css { get; set; }

            [Serializable]
            public class Js
            {
                public Js() {
                    this.CompressionType = CompressionType.Standard;
                    this.DisableOptimizations = false;
                    this.Encoding = Encoding.Default;
                    this.IgnoreEval = false;
                    this.LineBreakPosition = -1;
                    this.ObfuscateJavascript = true;
                    this.PreserveAllSemicolons = false;
                    this.ThreadCulture = CultureInfo.InvariantCulture;
                }
                public Js(Js YuiJsSettings)
                {
                    this.CompressionType = YuiJsSettings.CompressionType;
                    this.DisableOptimizations = YuiJsSettings.DisableOptimizations;
                    this.Encoding = YuiJsSettings.Encoding;
                    this.IgnoreEval = YuiJsSettings.IgnoreEval;
                    this.LineBreakPosition = YuiJsSettings.LineBreakPosition;
                    this.ObfuscateJavascript = YuiJsSettings.ObfuscateJavascript;
                    this.PreserveAllSemicolons = YuiJsSettings.PreserveAllSemicolons;
                    this.ThreadCulture = YuiJsSettings.ThreadCulture;
                }

                /// <summary>
                /// Standard (default) | None.  None => Concatenate files
                /// </summary>
                public CompressionType CompressionType { get; set; }

                /// <summary>
                /// Default is 'false'
                /// </summary>
                public bool DisableOptimizations { get; set; }

                /// <summary>
                /// Default is 'Encoding.Default'
                /// </summary>
                public Encoding Encoding { get; set; }

                /// <summary>
                /// True => compress any functions that contain 'eval'. 
                /// Default is False, which means a function that contains 'eval' will NOT be compressed.
                /// It's deemed risky to compress a function containing 'eval'. That said,
                /// if the usages are deemed safe this check can be disabled by setting this value to True.
                /// </summary>
                public bool IgnoreEval { get; set; }

                /// <summary>
                /// The position where a line feed is appended when the next semicolon is reached. 
                /// Default is -1 (never add a line break).
                /// 0 (zero) means add a line break after every semicolon. (This might help with debugging troublesome files).
                /// </summary>
                public int LineBreakPosition { get; set; }

                /// <summary>
                /// True (default) | False.  True => Obfuscate function and variable names
                /// </summary>
                public bool ObfuscateJavascript { get; set; }

                /// <summary>
                /// True | False (default).  True => preserve redundant semicolons (e.g. after a '}'
                /// </summary>
                public bool PreserveAllSemicolons { get; set; }

                /// <summary>
                /// The culture you want the thread to run under. 
                /// This affects the treatment of numbers etc - e.g. 9.00 could be output as 9,00.
                /// Default value is the Invariant Culture
                /// </summary>
                public CultureInfo ThreadCulture { get; set; }

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

            [Serializable]
            public class CSS
            {
                public CSS()
                {
                    this.CompressionType = Yahoo.Yui.Compressor.CompressionType.Standard;
                    this.LineBreakPosition = -1;
                    this.RemoveComments = true;
                }
                public CSS(CSS YuiCssSettings)
                {
                    this.CompressionType = YuiCssSettings.CompressionType;
                    this.LineBreakPosition = YuiCssSettings.LineBreakPosition;
                    this.RemoveComments = YuiCssSettings.RemoveComments;
                }

                /// <summary>
                /// Standard (default) | None.  None => Concatenate files only.
                /// </summary>
                public CompressionType CompressionType { get; set; }

                /// <summary>
                /// The position where a line feed is appended when the next semicolon is reached. 
                /// Default is -1 (never add a line break).
                /// 0 (zero) means add a line break after every semicolon. (This might help with debugging troublesome files).
                /// </summary>
                public int LineBreakPosition { get; set; }

                /// <summary>
                /// False | True (default).  
                /// Set False if you wish to preserve css comments.  
                /// True will remove them except ones marked with "!"
                /// </summary>
                public bool RemoveComments { get; set; }

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
        public YUICompressionSettings GlobalYuiCompressionSettings { get; set; }

        internal bool IsDebugMode()
        {
            if (_instance.ForceDebugMode)
                return true;

            if (_instance.ForceReleaseMode)
                return false;

            return StaticConfiguration.IsRunningDebug;
        }
    }
}
