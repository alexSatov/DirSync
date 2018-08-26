using System;
using System.Diagnostics;
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
                    .WithParsed(o => Execute(o, new FileSyncAsync(log)));
            }
            catch (UnauthorizedAccessException e)
            {
                log.Error($"{e.Message} (try launch with administrator rights)");
            }
            catch (Exception e)
            {
                log.Error($"Unknown error:\r\n{e}");
                Environment.Exit(-1);
            }
        }

        private static void Execute(Options options, BaseFileSync fileSync)
        {
            if (options.ShowSyncInfo)
            {
                ShowSyncInfo(fileSync.GetDirsSyncInfo(options.Source, options.Target));
                return;
            }

            var sw = new Stopwatch();
            sw.Start();

            var syncInfo = fileSync.SyncDirs(options.Source, options.Target);

            sw.Stop();

            ShowSyncResultStatistics(syncInfo, sw.ElapsedMilliseconds);
        }

        private static void ShowSyncResultStatistics(SyncInfo syncInfo, long elapsedMilliseconds)
        {
            if (syncInfo == null)
            {
                log.Info("Synchronization isn't started");
                return;
            }

            if (!syncInfo.NeedToSync)
            {
                log.Info("Already up to date");
                return;
            }

            var deletedFilesCount = syncInfo.FilesToDelete.Length;
            var addedFilesCount = syncInfo.FilesToAdd.Length;
            var replacedFilesCount = syncInfo.FilesToReplace.Length;

            if (deletedFilesCount > 0) log.Info($"\r\nDeleted files count: {deletedFilesCount}", ConsoleColor.Red);
            if (addedFilesCount > 0) log.Info($"\r\nAdded files count: {addedFilesCount}", ConsoleColor.Green);
            if (replacedFilesCount > 0) log.Info($"\r\nReplaced files count: {replacedFilesCount}", ConsoleColor.Cyan);

            log.Info($"\r\nTotal changed files count: {deletedFilesCount + addedFilesCount + replacedFilesCount}");
            log.Info($"\r\nSynchronization completed in {TimeSpan.FromMilliseconds(elapsedMilliseconds)}");
        }

        private static void ShowSyncInfo(SyncInfo syncInfo)
        {
            if (syncInfo == null) return;

            if (syncInfo.FilesToDelete.Length > 0)
                log.Info(
                    $"{syncInfo.FilesToDelete.Length} files to delete:\r\n{string.Join("\r\n", syncInfo.FilesToDelete)}\r\n",
                    ConsoleColor.Red);

            if (syncInfo.FilesToAdd.Length > 0)
                log.Info(
                    $"{syncInfo.FilesToAdd.Length} files to add:\r\n{string.Join("\r\n", syncInfo.FilesToAdd)}\r\n",
                    ConsoleColor.Green);

            if (syncInfo.FilesToReplace.Length > 0)
                log.Info(
                    $"{syncInfo.FilesToReplace.Length} files to replace:\r\n{string.Join("\r\n", syncInfo.FilesToReplace)}\r\n",
                    ConsoleColor.Cyan);
        }
    }
}
