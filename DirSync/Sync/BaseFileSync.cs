using System.Collections.Generic;
using System.IO;
using System.Linq;
using DirSync.Log;

namespace DirSync.Sync
{
    public abstract class BaseFileSync
    {
        protected readonly ILog Log;

        protected BaseFileSync(ILog log)
        {
            Log = log;
        }

        public abstract SyncInfo SyncDirs(string source, string target);
        public abstract SyncInfo GetDirsSyncInfo(string source, string target);

        protected void DeleteFiles(string target, IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                File.Delete(Path.Combine(target, filename));
                Log.DeleteInfo(filename);
            }
        }

        protected void AddFiles(string source, string target, IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                var targetPath = Path.Combine(target, filename);
                var targetDir = Path.GetDirectoryName(targetPath);

                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                File.Copy(Path.Combine(source, filename), targetPath);
                Log.AddInfo(filename);
            }
        }

        protected void ReplaceFiles(string source, string target, IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                File.Copy(Path.Combine(source, filename), Path.Combine(target, filename), true);
                Log.ReplaceInfo(filename);
            }
        }

        protected bool IsSourceExist(string source)
        {
            if (Directory.Exists(source)) return true;

            Log.Error($"Source directory \"{source}\" doesn't exist");

            return false;
        }

        protected bool SameDirs(string source, string target)
        {
            if (source != target) return false;

            Log.Warn("Source and target directories are the same (you shouldn't use copypaste)");

            return true;
        }

        protected void CreateTargetIfNotExist(string target)
        {
            if (Directory.Exists(target)) return;

            Log.Info($"Creating directory \"{target}\"");
            Directory.CreateDirectory(target);
        }

        protected static IEnumerable<string> GetAllFilenames(string dir, string prefix = "")
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

        protected static IEnumerable<string> FilterSameFiles(string source, string target, IEnumerable<string> filenames)
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
