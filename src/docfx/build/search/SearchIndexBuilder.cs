// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Microsoft.Docs.Build
{
    internal class SearchIndexBuilder
    {
        private readonly Config _config;
        private readonly ErrorBuilder _errors;
        private readonly MetadataProvider _metadataProvider;
        private readonly ConcurrentDictionary<FilePath, SearchIndexItem> _searchIndex = new ConcurrentDictionary<FilePath, SearchIndexItem>();

        public SearchIndexBuilder(Config config, ErrorBuilder errors, MetadataProvider metadataProvider)
        {
            _config = config;
            _errors = errors;
            _metadataProvider = metadataProvider;
        }

        public void SetTitle(Document file, string? title)
        {
            if (string.IsNullOrEmpty(title) || _config.SearchEngine != SearchEngineType.Lunr || NoIndex(file.FilePath))
            {
                return;
            }

            _searchIndex.GetOrAdd(file.FilePath, _ => new SearchIndexItem(file.SiteUrl)).Title = title;
        }

        public void SetBody(Document file, string? body)
        {
            if (string.IsNullOrEmpty(body) || _config.SearchEngine != SearchEngineType.Lunr || NoIndex(file.FilePath))
            {
                return;
            }

            _searchIndex.GetOrAdd(file.FilePath, _ => new SearchIndexItem(file.SiteUrl)).Body = body;
        }

        public string? Build()
        {
            if (_searchIndex.IsEmpty)
            {
                return null;
            }

            var documents = JToken.FromObject(_searchIndex.Values);
            var js = JavaScriptEngine.Create();
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "data/scripts/lunr.interop.js");

            return js.Run(scriptPath, "transform", documents).ToString();
        }

        private bool NoIndex(FilePath file)
        {
            var metadata = _metadataProvider.GetMetadata(_errors, file);
            if (metadata.Robots != null && metadata.Robots.Contains("noindex", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}