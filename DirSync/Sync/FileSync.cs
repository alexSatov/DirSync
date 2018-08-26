using System.Collections.Generic;
using System.Linq;
using DirSync.Log;

namespace DirSync.Sync
{
    public class FileSync : BaseFileSync
    {
        public FileSync(ILog log) : base(log)
        {
        }

        public override SyncInfo SyncDirs(string source, string target)
        {
            var syncInfo = GetDirsSyncInfo(source, target);

            if (syncInfo == null) return null;
            if (!syncInfo.NeedToSync) return syncInfo;

            Log.Info($"Synchronizing \"{target}\" with \"{source}\"\r\n");

            DeleteFiles(target, syncInfo.FilesToDelete);
            AddFiles(source, target, syncInfo.FilesToAdd);
            ReplaceFiles(source, target, syncInfo.FilesToReplace);

            return syncInfo;
        }

        public override SyncInfo GetDirsSyncInfo(string source, string target)
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
    }
}
