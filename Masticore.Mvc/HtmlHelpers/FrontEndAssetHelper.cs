using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace Masticore.Mvc.HtmlHelpers
{
    public class FrontEndAssets
    {
        public List<string> Css { get; set; }
        public List<string> Js { get; set; }
    }

    public interface IFrontEndAssetCollection : IDictionary<string, FrontEndAssets> { }
    public class FrontEndAssetCollection : Dictionary<string, FrontEndAssets>, IFrontEndAssetCollection { }

    public class FrontEndManifest
    {
        public string DevServer { get; set; }
        public bool HasPublicPathArg { get; set; }
        public FrontEndAssetCollection EntryPoints { get; set; }
    }

    public static class FrontEndAssetHelper
    {
        private static string _manifestPath = "Scripts\\dist\\lt\\manifest.json";
        private static string _publicPath = "";

        /// <summary>
        /// Configure the public path and manifest path for front end assets. Null values will be ignored.
        /// </summary>
        /// <param name="publicPath">Public path for assets (change for CDN, default: "/" for same server domain)</param>
        /// <param name="manifestPath">(Windows) server relative path to the manifest file. (default: "Scripts\\dist\\lt\\manifest.json")</param>
        public static void Configure(string publicPath = null, string manifestPath = null)
        {
            if (publicPath != null)
            {
                _publicPath = publicPath;
            }

            if (manifestPath != null)
            {
                _manifestPath = manifestPath;
            }
        }

        private static JObject ReadManifest()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _manifestPath);
            var fileContent = File.ReadAllText(dir, Encoding.UTF8);
            return JObject.Parse(fileContent);
        }
        private const string PublicPathArg = "{0}";
        private static string EncodedPublicPathArg => WebUtility.UrlEncode(PublicPathArg);

        private static IFrontEndAssetCollection CreateCollection()
        {
            var manifest = ReadManifest().ToObject<FrontEndManifest>();
            var injectDevServer = !string.IsNullOrWhiteSpace(manifest.DevServer);

            if (!injectDevServer && !manifest.HasPublicPathArg) return manifest.EntryPoints;

            foreach (var entry in manifest.EntryPoints)
            {
                if (manifest.HasPublicPathArg)
                {
                    for (var i = 0; i < entry.Value.Css.Count; i++)
                    {
                        entry.Value.Css[i] = entry.Value.Css[i].Replace(EncodedPublicPathArg, PublicPathArg);
                    }

                    for (var i = 0; i < entry.Value.Js.Count; i++)
                    {
                        entry.Value.Js[i] = entry.Value.Js[i].Replace(EncodedPublicPathArg, PublicPathArg);
                    }
                }

                if (injectDevServer)
                {
                    entry.Value.Js.Insert(0, manifest.DevServer);
                }

            }

            return manifest.EntryPoints;
        }

        public static Lazy<IFrontEndAssetCollection> LazyFrontEndAssetCollection => new Lazy<IFrontEndAssetCollection>(CreateCollection);

        /// <summary>
        /// Gets the front end asset collection. The collection is statically cached for reuse, unless debug is enabled
        /// </summary>
        public static IFrontEndAssetCollection FrontEndAssetCollection =>
            DebugHelper.IsDebug ? CreateCollection() : LazyFrontEndAssetCollection.Value;

        private static IHtmlString RenderStrings(IList<string> values, Func<string, string> fn) => MvcHtmlString.Create(values != null && values.Any() ? string.Join("\n", values.Select(fn)) : "");

        private static string LinkTemplate(string href) =>
            $"<link rel=\"stylesheet\" href=\"{string.Format(href, _publicPath)}\">";

        private static string ScriptTemplate(string src) =>
            $"<script type=\"text/javascript\" src=\"{string.Format(src, _publicPath)}\"></script>";

        public static IHtmlString RenderStylesForEntry(this HtmlHelper helper, string entry) => RenderStrings(FrontEndAssetCollection[entry].Css, LinkTemplate);

        public static IHtmlString RenderScriptsForEntry(this HtmlHelper helper, string entry) => RenderStrings(FrontEndAssetCollection[entry].Js, ScriptTemplate);

    }
}
