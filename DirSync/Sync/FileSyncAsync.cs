using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DirSync.Logging;

namespace DirSync.Sync
{
    public class FileSyncAsync : BaseFileSync
    {
        public FileSyncAsync(ILog log) : base(log)
        {
        }

        public override SyncInfo SyncDirs(string source, string target)
        {
            return SyncDirsAsync(source, target).Result;
        }

        public override SyncInfo GetDirsSyncInfo(string source, string target)
        {
            return GetDirsSyncInfoAsync(source, target).Result;
        }

        public async Task<SyncInfo> SyncDirsAsync(string source, string target)
        {
            var syncInfo = await GetDirsSyncInfoAsync(source, target);

            if (syncInfo == null) return null;
            if (!syncInfo.NeedToSync) return syncInfo;

            Log.Info($"Synchronizing \"{target}\" with \"{source}\"\r\n");

            await Task.WhenAll(
                Task.Run(() => DeleteFiles(target, syncInfo.FilesToDelete)),
                AddFilesAsync(source, target, syncInfo.FilesToAdd),
                ReplaceFilesAsync(source, target, syncInfo.FilesToReplace));

            return syncInfo;
        }

        public async Task<SyncInfo> GetDirsSyncInfoAsync(string source, string target)
        {
            if (!IsSourceExist(source)) return null;
            if (AreSameDirs(source, target)) return null;

            CreateTargetIfNotExist(target);

            var tasks = new[]
            {
                Task.Run(() => GetAllFilenames(source)),
                Task.Run(() => GetAllFilenames(target))
            };

            await Task.WhenAll(tasks);

            return new SyncInfo(tasks[0].Result.ToList(), tasks[1].Result.ToList(),
                f => FilterSameFiles(source, target, f));
        }

        private async Task AddFilesAsync(string source, string target, IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                var targetPath = Path.Combine(target, filename);
                var targetDir = Path.GetDirectoryName(targetPath);

                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                await CopyFileAsync(Path.Combine(source, filename), targetPath);
                Log.AddInfo(filename);
            }
        }

        private async Task ReplaceFilesAsync(string source, string target, IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                await CopyFileAsync(Path.Combine(source, filename), Path.Combine(target, filename));
                Log.ReplaceInfo(filename);
            }
        }

        private static async Task CopyFileAsync(string from, string to)
        {
            using (var source = File.OpenRead(from))
            {
                using (var target = new FileStream(to, FileMode.Create, FileAccess.Write))
                {
                    await source.CopyToAsync(target);
                }
            }
        }
    }
}
