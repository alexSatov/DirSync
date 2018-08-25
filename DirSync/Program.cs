using System;
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
            catch (Exception e)
            {
                log.Error($"Unknown error:\r\n{e}");
                Environment.Exit(-1);
            }
        }

        private static void Execute(Options options)
        {
            var dirSync = new FileSynchronizer(log);
            dirSync.SyncDirs(options.Source, options.Target);
        }
    }
}
