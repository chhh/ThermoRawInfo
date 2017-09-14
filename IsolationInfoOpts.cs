using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace ThermoRawInfo
{
    [Verb("isolation", HelpText = "Print isolation info for MS2+ scans")]
    public class IsolationInfoOpts : IOptions
    {
        public string File { get; set; }

        [Option('n', "num", HelpText = "Scan number range. One or two numbers separated by a space. " +
                                       "If one number is given, a single scan is read, if two, they are considered an inclusive range.", Min = 1, Max = 2)]
        public IList<int> ScanNum { get; set; }
        
    }
}
