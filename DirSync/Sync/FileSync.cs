using System.Linq;
using DirSync.Logging;

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
            if (AreSameDirs(source, target)) return null;

            CreateTargetIfNotExist(target);

            return new SyncInfo(GetAllFilenames(source).ToList(), GetAllFilenames(target).ToList(),
                f => FilterSameFiles(source, target, f));
        }
    }
}
