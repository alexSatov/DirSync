using System;
using System.Collections.Generic;
using CommandLine;
using DirSync.Log;

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
                    .WithParsed(Execute)
                    .WithNotParsed(HandleErrors);
            }
            catch (Exception e)
            {
                log.Error($"Unknown error:\r\n{e}");
                Environment.Exit(-1);
            }
        }

        private static void Execute(Options options)
        {
            log.Info($"Start synchronizing {options.Target} with {options.Source}");
        }

        private static void HandleErrors(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                log.Error($"Arguments parsing error:\r\n{error}");
            }
        }
    }
}
