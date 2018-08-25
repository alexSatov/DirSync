using System.Collections.Generic;
using System.IO;
using System.Linq;
using DirSync.Log;

namespace DirSync.Sync
{
    public class FileSynchronizer
    {
        private ILog log;

        public FileSynchronizer(ILog log)
        {
            this.log = log;
        }

        public void SyncDirs(string source, string target)
        {
            if (!IsSourceExist(source)) return;
            
            CreateTargetIfNotExist(target);
            log.Info($"Synchronizing \"{target}\" with \"{source}\"");

            var syncInfo = GetDirsSyncInfo(source, target);
        }

        public SyncInfo GetDirsSyncInfo(string source, string target)
        {
            var sourceFilenames = new HashSet<string>(GetAllFilenames(new DirectoryInfo(source)));
            var targetFilenames = new HashSet<string>(GetAllFilenames(new DirectoryInfo(target)));

            return new SyncInfo
            {
                ToDelete = targetFilenames.Except(sourceFilenames),
                ToAdd = sourceFilenames.Except(targetFilenames),
                ToCompare = sourceFilenames.Intersect(targetFilenames)
            };
        }

        private int DeleteFiles(IEnumerable<FileInfo> files)
        {
            var count = 0;

            foreach (var file in files)
            {
                count++;
                file.Delete();
            }

            return count;
        }

        private bool IsSourceExist(string source)
        {
            if (Directory.Exists(source)) return true;

            log.Error($"Source directory \"{source}\" doesn't exist");

            return false;
        }

        private void CreateTargetIfNotExist(string target)
        {
            if (Directory.Exists(target)) return;

            log.Info($"Creating directory \"{target}\"");
            Directory.CreateDirectory(target);
        }

        private static IEnumerable<string> GetAllFilenames(DirectoryInfo dir, string prefix = "")
        {
            var filenames = new HashSet<string>(dir.EnumerateFiles().Select(f => Path.Combine(prefix, f.Name)));

            foreach (var dirInfo in dir.EnumerateDirectories())
            {
                var innerFilenames = GetAllFilenames(dirInfo, dirInfo.Name);
                filenames.UnionWith(innerFilenames);
            }

            return filenames;
        }
    }
}
