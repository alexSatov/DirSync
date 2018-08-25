using CommandLine;

namespace DirSync
{
    public class Options
    {
        [Option('s', "source", Required = true, HelpText = "Source directory for synchronizing")]
        public string Source { get; set; }

        [Option('t', "target", Required = true, HelpText = "Target directory which will be synchronized with source directory")]
        public string Target { get; set; }
    }
}
