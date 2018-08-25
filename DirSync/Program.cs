using System;
using System.Diagnostics;
using System.Linq;
using CommandLine;
using DirSync.Log;
using DirSync.Sync;

namespace DirSync
{
    public class Program
    {
        private static readonly ILog log = new SyncLog();

        public static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(Execute);
            }
            catch (UnauthorizedAccessException e)
            {
                log.Error($"{e.Message} (try launch with administator rights)");
            }
            catch (Exception e)
            {
                log.Error($"Unknown error:\r\n{e}");
                Environment.Exit(-1);
            }
        }

        private static void Execute(Options options)
        {
            var dirSync = new FileSynchronizer(log);

            if (options.ShowSyncInfo)
            {
                ShowSyncInfo(dirSync.GetDirsSyncInfo(options.Source, options.Target));
                return;
            }

            var sw = new Stopwatch();
            sw.Start();

            var syncInfo = dirSync.SyncDirs(options.Source, options.Target);
            sw.Stop();

            if (syncInfo == null)
            {
                log.Info("Synchronization isn't started");
            }

            log.Info($"Synchronization completed in {TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds)}");
        }

        private static void ShowSyncInfo(SyncInfo syncInfo)
        {
            if (syncInfo == null) return;

            var filesToDelete = syncInfo.FilesToDelete.ToArray();
            var filesToAdd = syncInfo.FilesToAdd.ToArray();
            var filesToReplace = syncInfo.FilesToReplace.ToArray();

            if (filesToDelete.Length > 0)
                log.Info($"{filesToDelete.Length} files to delete:\r\n{string.Join("\r\n", filesToDelete)}\r\n", ConsoleColor.Red);

            if (filesToAdd.Length > 0)
                log.Info($"{filesToAdd.Length} files to add:\r\n{string.Join("\r\n", filesToAdd)}\r\n", ConsoleColor.Green);

            if (filesToReplace.Length > 0)
                log.Info($"{filesToReplace.Length} files to replace:\r\n{string.Join("\r\n", filesToReplace)}\r\n", ConsoleColor.Cyan);
        }
    }
}
