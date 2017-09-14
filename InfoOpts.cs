using CommandLine;

namespace ThermoRawInfo
{
    [Verb("info", HelpText = "Display info about a file")]
    public class InfoOpts : IOptions
    {
        public string File { get; set; }
    }
    
}
