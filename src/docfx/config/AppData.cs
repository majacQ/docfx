// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.Docs.Build
{
    internal static class AppData
    {
        private static readonly string s_root = GetAppDataRoot();

        // For testing purpose
        internal static Func<string> GetCachePath;
        internal static Func<string> GetStatePath;

        public static string GitRoot => Path.Combine(s_root, "git6");

        public static string DownloadsRoot => Path.Combine(s_root, "downloads2");

        public static string MutexRoot => Path.Combine(s_root, "mutex");

        public static string CacheRoot => GetCachePath?.Invoke() ?? EnvironmentVariable.CachePath ?? Path.Combine(s_root, "cache");

        public static string StateRoot => GetStatePath?.Invoke() ?? EnvironmentVariable.StatePath ?? Path.Combine(s_root, "state");

        public static string GlobalConfigPath => GetGlobalConfigPath();

        public static string GitHubUserCachePath => Path.Combine(CacheRoot, "github-users.json");

        public static string MicrosoftGraphCachePath => Path.Combine(CacheRoot, "microsoft-graph.json");

        public static string GetFileDownloadDir(string url)
        {
            return Path.Combine(DownloadsRoot, PathUtility.UrlToShortName(url));
        }

        public static string GetDependencyLockFile(string docsetPath, string locale)
        {
            return PathUtility.NormalizeFile(
                    Path.Combine(s_root, "lock", PathUtility.UrlToShortName(docsetPath), locale ?? "", ".lock.json"));
        }

        public static string GetCommitCachePath(string remote)
        {
            return Path.Combine(CacheRoot, "commits", HashUtility.GetMd5Hash(remote));
        }

        public static string GetCommitBuildTimePath(string remote, string branch)
        {
            return Path.Combine(
                StateRoot, "history", $"build_history_{HashUtility.GetMd5Guid(remote)}_{HashUtility.GetMd5Guid(branch)}.json");
        }

        /// <summary>
        /// Get the global configuration path, default is under <see cref="s_root"/>
        /// </summary>
        private static string GetGlobalConfigPath()
        {
            return EnvironmentVariable.GlobalConfigPath != null
                ? Path.GetFullPath(EnvironmentVariable.GlobalConfigPath)
                : PathUtility.FindYamlOrJson(s_root, "docfx");
        }

        /// <summary>
        /// Get the application cache root dir, default is under user proflie dir.
        /// User can set the DOCFX_APPDATA_PATH environment to change the root
        /// </summary>
        private static string GetAppDataRoot()
        {
            return EnvironmentVariable.AppDataPath != null
                ? Path.GetFullPath(EnvironmentVariable.AppDataPath)
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".docfx");
        }
    }
}