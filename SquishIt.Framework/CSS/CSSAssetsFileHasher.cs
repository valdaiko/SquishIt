﻿using System;
using System.IO;
using SquishIt.Framework.Resolvers;
using SquishIt.Framework.Utilities;
using SquishIt.Framework.Web;

namespace SquishIt.Framework.CSS
{
    public class CSSAssetsFileHasher : ICSSAssetsFileHasher
    {
        protected readonly string HashQueryStringKeyName;
        protected readonly IFileResolver fileResolver;
        protected readonly IHasher Hasher;
        private readonly IPathTranslator pathTranslator;
        private readonly IHttpUtility httpUtility;

        public CSSAssetsFileHasher(string hashQueryStringKeyName, IFileResolver fileResolver, IHasher hasher, IPathTranslator pathTranslator, IHttpUtility httpUtility)
        {
            if (httpUtility == null)
            {
                throw new ArgumentNullException("httpUtility");
            }

            HashQueryStringKeyName = hashQueryStringKeyName;
            this.fileResolver = fileResolver;
            Hasher = hasher;
            this.pathTranslator = pathTranslator;
            this.httpUtility = httpUtility;
        }

        public string AppendFileHash(string cssFilePath, string url)
        {
            if (url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                return url;
            }

            var assetFilePath = GetAssetFilePath(cssFilePath, url);
            var fileInfo = new FileInfo(assetFilePath);

            if (!fileInfo.Exists)
            {
                return url;
            }

            if (!string.IsNullOrEmpty(HashQueryStringKeyName))
            {
                var hash = Hasher.GetHash(fileInfo);
                url = AppendQueryStringPairValue(url, HashQueryStringKeyName, hash);
            }

            return url;
        }

        string GetAssetFilePath(string cssFilePath, string url)
        {
            var queryStringPosition = url.IndexOf('?');

            if (queryStringPosition > -1)
            {
                url = url.Substring(0, queryStringPosition);
            }

            if (Platform.Unix)
            {
                url = url.TrimStart('/');
            }

            var resolvedUrl = string.Empty;

            var urlUri = new Uri(url, UriKind.RelativeOrAbsolute);

            if (!urlUri.IsAbsoluteUri)
            {
                if (!url.StartsWith("/"))
                {
                    var resolvedPath = Path.GetDirectoryName(cssFilePath);
                    if (Platform.Unix)
                    {
                        resolvedPath = resolvedPath.Replace("file:", "");
                    }

                    var outputUri = new Uri(resolvedPath + "/", UriKind.Absolute);

                    var resolvedSourcePath = new Uri(outputUri, urlUri);
                    resolvedUrl = resolvedSourcePath.LocalPath;
                }
                else
                {
                    resolvedUrl = pathTranslator.ResolveAppRelativePathToFileSystem(url);
                }

                return fileResolver.ResolveFilename(resolvedUrl);
            }

            return urlUri.LocalPath;
        }

        /// <summary>
        /// Append a query string pair value to a url
        /// </summary>
        /// <param name="url">The url to add query string pair value value to.</param>
        /// <param name="key">The key to use.</param>
        /// <param name="value">The value to use.</param>
        /// <returns></returns>
        string AppendQueryStringPairValue(string url, string key, string value)
        {
            var path = url;
            var queryString = string.Empty;

            var queryStringPosition = url.IndexOf('?');

            if (queryStringPosition > -1)
            {
                path = url.Substring(0, queryStringPosition);
                queryString = url.Substring(queryStringPosition);
            }

            var querystring = httpUtility.ParseQueryString(queryString);

            querystring.Add(key, value);

            var querystringwithAppendedValue = FlattenedQueryString(querystring);

            if (!string.IsNullOrEmpty(querystringwithAppendedValue))
            {
                querystringwithAppendedValue = "?" + querystringwithAppendedValue;
            }

            return path + querystringwithAppendedValue;
        }

        //workaround for mono bug - queryString.ToString() above was returning "System.Collections.Specialized.NameValueCollection"
        static string FlattenedQueryString(System.Collections.Specialized.NameValueCollection queryString)
        {
            var output = new System.Text.StringBuilder();
            for (int i = 0; i < queryString.Count; i++)
            {
                if (i > 0) output.Append("&");
                output.Append(queryString.AllKeys[i]);
                output.Append("=");
                output.Append(queryString[i]);
            }
            return output.ToString();
        }
    }
}
