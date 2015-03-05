
using MemgrindDifferencingEngine.DataModel;
using MemgrindDifferencingEngine.Parsing;
using System.Collections.Generic;
using System.IO;
namespace MemgrindDifferencingEngine
{
    /// <summary>
    /// Parse the mem-grind log file
    /// </summary>
    public static class MemgrindLogParser
    {
        /// <summary>
        /// Parse a log file that contains memgrind messages
        /// </summary>
        /// <param name="input"></param>
        /// <returns>an object that contains all memgrind info</returns>
        public static MemgrindInfo Parse(FileInfo input)
        {
            var result = new MemgrindInfo();

            // Basic info
            result.Description = input.Name;

            // Load up the summary parse items
            var summaryParseItems = new ParseAfterLine("==16280== LEAK SUMMARY:")
            {
                new ParseSingleLineItem("==16280==", "definitely lost: (?<bytes>[0-9,]+) bytes in (?<blocks>[0-9,]+) blocks", re => { result.LostBlocks = re.AsMemgrindNumber("blocks"); result.LostBytes = re.AsMemgrindNumber("bytes");})
            };
            var parseItems = new List<ParseItemBase>()
            {
                summaryParseItems
            };

            // Now the main parser loop. These files can be big, so we need to stream them. And they are going
            // to suck up a great deal of memory.
            foreach (var line in input.FileIterator())
            {
                // Look for the summary information at the end.
                foreach (var p in parseItems)
                {
                    p.Process(line);
                }

            }

            return result;
        }
    }
}

#if false
==16280== LEAK SUMMARY:
==16280==    definitely lost: 882,960 bytes in 25,957 blocks
==16280==    indirectly lost: 43,638,695 bytes in 181,383 blocks
==16280==      possibly lost: 39,136,849 bytes in 503,301 blocks
==16280==    still reachable: 207,534,176 bytes in 273,479 blocks
==16280==         suppressed: 69,028,905 bytes in 625,728 blocks
==16280== 
==16280== For counts of detected and suppressed errors, rerun with: -v
==16280== ERROR SUMMARY: 21975 errors from 8507 contexts (suppressed: 1722319 from 5599)
#endif