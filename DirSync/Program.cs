using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommandLine;
using DirSync.Extensions;
using DirSync.Logging;
using DirSync.Sync;
using DirSyncAndroid;

namespace DirSync
{
    public class Program
    {
        private static readonly ILog log = new ConsoleLog();

        public static void Main(string[] args)
        {
            try
            {
                Test();
                Parser.Default.ParseArguments<Options>(args).WithParsed(o => Execute(o, new FileSyncAsync(log)));
            }
            catch (UnauthorizedAccessException e)
            {
                log.Error($"{e.Message} (try launch with administrator rights)");
                Environment.Exit(-1);
            }
            catch (Exception e)
            {
                log.Error($"Unknown error:\r\n{e}");
                Environment.Exit(-1);
            }
        }

        private static void Test()
        {
            foreach (var deviceId in AndroidDeviceProvider.GetDeviceIds())
            {
                Console.WriteLine(deviceId);
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
                log.Info("Already up-to-date");
                return;
            }

            if (syncInfo.FilesToDelete.Count > 0)
                log.Info($"\r\nDeleted files count: {syncInfo.FilesToDelete.Count}", ConsoleColor.Red);

            if (syncInfo.FilesToAdd.Count > 0)
                log.Info($"\r\nAdded files count: {syncInfo.FilesToAdd.Count}", ConsoleColor.Green);

            if (syncInfo.FilesToReplace.Count > 0)
                log.Info($"\r\nReplaced files count: {syncInfo.FilesToReplace.Count}", ConsoleColor.Cyan);

            var total = syncInfo.FilesToDelete.Count + syncInfo.FilesToAdd.Count + syncInfo.FilesToReplace.Count;

            log.Info($"\r\nTotal changed files count: {total}");
            log.Info($"\r\nSynchronization completed in {TimeSpan.FromMilliseconds(elapsedMilliseconds)}");
        }

        private static void ShowSyncInfo(SyncInfo syncInfo)
        {
            if (syncInfo == null) return;

            void Log(IReadOnlyCollection<string> files, string action, ConsoleColor color)
            {
                if (files.Count > 0)
                    log.Info($"{files.Count} files to {action}:\r\n{files.ToLinesText()}\r\n", color);
            }

            Log(syncInfo.FilesToDelete, "delete", ConsoleColor.Red);
            Log(syncInfo.FilesToAdd, "add", ConsoleColor.Green);
            Log(syncInfo.FilesToReplace, "replace", ConsoleColor.Cyan);
        }
    }
}
