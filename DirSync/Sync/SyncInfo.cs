using System;
using System.Collections.Generic;
using System.Linq;

namespace DirSync.Sync
{
    public class SyncInfo
    {
        public List<string> FilesToDelete { get; }
        public List<string> FilesToAdd { get; }
        public List<string> FilesToReplace { get; }

        public bool NeedToSync => FilesToDelete.Count > 0 || FilesToAdd.Count > 0 || FilesToReplace.Count > 0;

        public SyncInfo(IReadOnlyList<string> sourceFilenames, IReadOnlyList<string> targetFilenames,
            Func<IEnumerable<string>, IEnumerable<string>> equalityFilter)
        {
            FilesToDelete = targetFilenames.Except(sourceFilenames).ToList();
            FilesToAdd = sourceFilenames.Except(targetFilenames).ToList();
            FilesToReplace = equalityFilter(sourceFilenames.Intersect(targetFilenames)).ToList();
        }
    }
}
