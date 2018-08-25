using System.Collections.Generic;

namespace DirSync.Sync
{
    public class SyncInfo
    {
        public string[] FilesToDelete { get; set; }
        public string[] FilesToAdd { get; set; }
        public string[] FilesToReplace { get; set; }

        public bool NeedToSync => FilesToDelete.Length > 0 || FilesToAdd.Length > 0 || FilesToReplace.Length > 0;
    }
}
