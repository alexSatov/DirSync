using System.Collections.Generic;

namespace DirSync.Sync
{
    public class SyncInfo
    {
        public IEnumerable<string> ToDelete { get; set; }
        public IEnumerable<string> ToAdd { get; set; }
        public IEnumerable<string> ToCompare { get; set; }
    }
}
