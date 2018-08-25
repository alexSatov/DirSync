using System.Collections.Generic;
using System.IO;
using System.Linq;
using DirSync.Log;

namespace DirSync.Sync
{
    public class FileSynchronizer
    {
        private readonly ILog log;

        public FileSynchronizer(ILog log)
        {
            this.log = log;
        }

        public SyncInfo SyncDirs(string source, string target)
        {
            var syncInfo = GetDirsSyncInfo(source, target);

            if (syncInfo == null) return null;

            log.Info($"Synchronizing \"{target}\" with \"{source}\"");

            DeleteFiles(target, syncInfo.FilesToDelete);
            AddFiles(source, target, syncInfo.FilesToAdd);
            ReplaceFiles(source, target, syncInfo.FilesToReplace);

            return syncInfo;
        }

        public SyncInfo GetDirsSyncInfo(string source, string target)
        {
            if (!IsSourceExist(source)) return null;
            if (SameDirs(source, target)) return null;

            CreateTargetIfNotExist(target);

            var sourceFilenames = new HashSet<string>(GetAllFilenames(source));
            var targetFilenames = new HashSet<string>(GetAllFilenames(target));

            return new SyncInfo
            {
                FilesToDelete = targetFilenames.Except(sourceFilenames).ToArray(),
                FilesToAdd = sourceFilenames.Except(targetFilenames).ToArray(),
                FilesToReplace = FilterSameFiles(source, target, sourceFilenames.Intersect(targetFilenames)).ToArray()
            };
        }

        private void DeleteFiles(string target, IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                File.Delete(Path.Combine(target, filename));
                log.DeleteInfo(filename);
            }
        }

        private void AddFiles(string source, string target, IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                var targetPath = Path.Combine(target, filename);
                var targetDir = Path.GetDirectoryName(targetPath);

                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                File.Copy(Path.Combine(source, filename), targetPath);
                log.AddInfo(filename);
            }
        }

        private void ReplaceFiles(string source, string target, IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                File.Copy(Path.Combine(source, filename), Path.Combine(target, filename), true);
                log.ReplaceInfo(filename);
            }
        }

        private bool IsSourceExist(string source)
        {
            if (Directory.Exists(source)) return true;

            log.Error($"Source directory \"{source}\" doesn't exist");

            return false;
        }

        private bool SameDirs(string source, string target)
        {
            if (source != target) return false;

            log.Warn("Source and target directories are the same (you shouldn't use copypaste)");

            return true;
        }

        private void CreateTargetIfNotExist(string target)
        {
            if (Directory.Exists(target)) return;

            log.Info($"Creating directory \"{target}\"");
            Directory.CreateDirectory(target);
        }

        private static IEnumerable<string> GetAllFilenames(string dir, string prefix = "")
        {
            var filenames = new HashSet<string>(
                    Directory.EnumerateFiles(dir).Select(f => Path.Combine(prefix, Path.GetFileName(f))));

            foreach (var dirPath in Directory.EnumerateDirectories(dir))
            {
                var innerFilenames = GetAllFilenames(dirPath, Path.Combine(prefix, Path.GetFileName(dirPath)));
                filenames.UnionWith(innerFilenames);
            }

            return filenames;
        }

        private static IEnumerable<string> FilterSameFiles(string source, string target, IEnumerable<string> filenames)
        {   //TODO: hash comparing
            foreach (var filename in filenames)
            {
                var sourceFile = new FileInfo(Path.Combine(source, filename));
                var targetFile = new FileInfo(Path.Combine(target, filename));

                if (sourceFile.Length != targetFile.Length) yield return filename;
            }
        }
    }
}
