﻿using CommandLine;

namespace DirSync
{
    public class Options
    {
        [Option('s', "source", Required = true, HelpText = "Source directory for synchronizing")]
        public string Source { get; set; }

        [Option('t', "target", Required = true, HelpText = "Target directory which will be synchronized with source directory (will be created if doesn't exist)")]
        public string Target { get; set; }

        [Option('i', "info", Required = false, HelpText = "Show sync info (without applying)")]
        public bool ShowSyncInfo { get; set; }
    }
}
