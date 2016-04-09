﻿using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Utilities;

namespace SquishIt.Mvc
{
    public static class HtmlHelperExtensions
    {
        private static readonly IMachineConfigReader _machineConfigReader = new MachineConfigReader();

        public static CSSBundle BundleCss(this HtmlHelper html, IDebugStatusReader debugStatusReader = null)
        {
            debugStatusReader = debugStatusReader ?? new DebugStatusReader(_machineConfigReader, new AspNet.Web.HttpContext(HttpContext.Current));
            var bundleCss = new CSSBundle(debugStatusReader);

            return bundleCss;
        }

        public static JavaScriptBundle BundleJavaScript(this HtmlHelper html, IDebugStatusReader debugStatusReader = null)
        {
            debugStatusReader = debugStatusReader ?? new DebugStatusReader(_machineConfigReader, new AspNet.Web.HttpContext(HttpContext.Current));
            return new JavaScriptBundle(debugStatusReader);
        }

        public static string ViewName(this HtmlHelper html)
        {
            var webPage = html.ViewDataContainer as WebPageBase;
            return webPage.VirtualPath;
        }
        public static void AddResources(this HtmlHelper html, params string[] resourceFiles)
        {
            AutoBundler.Current.AddResources(html.ViewName(),resourceFiles);
        }

        public static void AddStyleResources(this HtmlHelper html, params string[] resourceFiles)
        {
            AutoBundler.Current.AddStyleResources(html.ViewName(), resourceFiles);
        }

        public static void AddScriptResources(this HtmlHelper html, params string[] resourceFiles)
        {
            AutoBundler.Current.AddScriptResources(html.ViewName(), resourceFiles);
        }

        public static HtmlString ResourceLinks(this HtmlHelper html)
        {
            return new HtmlString(AutoBundler.Current.StyleResourceLinks + AutoBundler.Current.ScriptResourceLinks);
        }

        public static HtmlString StyleResourceLinks(this HtmlHelper html)
        {
            return new HtmlString(AutoBundler.Current.StyleResourceLinks);
        }

        public static HtmlString ScriptResourceLinks(this HtmlHelper html)
        {
            return new HtmlString(AutoBundler.Current.ScriptResourceLinks);
        }
    }
}
