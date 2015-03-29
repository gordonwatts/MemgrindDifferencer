
using MemgrindDifferencingEngine;
using System;
using System.IO;
using System.Linq;

namespace memgrind_diff
{
    class Program
    {
        /// <summary>
        /// Simple program to generate an excel file of differences between two memgrind files.
        /// Best run with same program or when the programs are very similar so it can collate differences.
        /// 
        /// Usage: memgrind_diff <file1> <file2>
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var opt = new CommandLineOptions();
            if (!CommandLine.Parser.Default.ParseArguments(args, opt))
            {
                Console.WriteLine("Usage: memgrind_diff [--min-size <n>] <file1> <file2>...");
                return;
            }

            // Parse the command line arguments
            var files = opt.InputFiles;

            // Get the info from each file.
            var info = files.Select(x => new FileInfo(x)).Select(x => MemgrindLogParser.Parse(x)).ToArray();

            // Dump the info
            foreach (var f in info)
            {
                Console.WriteLine("For file: {0}", f.Description);
                Console.WriteLine("  Lost blocks: {0}", f.LostBlocks);
                Console.WriteLine("  Lost bytes: {0}", f.LostBytes);
                Console.WriteLine("  Suppressed blocks: {0} {1}", f.SuppressedBlocks, f.SuppressedBytes);

                Console.WriteLine("  Saw {0} types of errors.", f.GrindDumpErrors.Count);
                Console.WriteLine("  Saw {0} loss records", f.DefinitelyLost.Count);
                Console.WriteLine("  Saw {0} possible loss records", f.PossiblyLost.Count);
                Console.WriteLine("  Saw {0} reachable records", f.StillReachable.Count);
            }

            // Dump to excel (only things that are larger than a 1KB leak).
            DumpToExcel.Dump(new FileInfo(@"leaks.xlsx"), info, opt.MinimumLeakSize);
        }
    }
}
