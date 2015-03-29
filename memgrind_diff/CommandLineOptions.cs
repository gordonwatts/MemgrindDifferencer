using CommandLine;
using System.Collections.Generic;

namespace memgrind_diff
{
    public class CommandLineOptions
    {
        /// <summary>
        /// Minimum leak size
        /// </summary>
        [Option('m', "min-size", DefaultValue = 1024, HelpText = "Minimum size of a leak")]
        public int MinimumLeakSize { get; set; }

        /// <summary>
        /// The files to process
        /// </summary>
        [ValueList(typeof(List<string>))]
        public IList<string> InputFiles { get; set; }
    }
}
