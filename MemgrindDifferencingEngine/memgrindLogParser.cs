
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
            var summaryParseItems = new ParseAfterLine("== LEAK SUMMARY:")
            {
                new ParseSingleLineItem("==", "definitely lost: (?<bytes>[0-9,]+) bytes in (?<blocks>[0-9,]+) blocks", re => { result.LostBlocks = re.AsMemgrindNumber("blocks"); result.LostBytes = re.AsMemgrindNumber("bytes");}),
                new ParseSingleLineItem("==", "indirectly lost: (?<bytes>[0-9,]+) bytes in (?<blocks>[0-9,]+) blocks", re => { result.IndirectlyLostBlocks = re.AsMemgrindNumber("blocks"); result.IndirectlyLostBytes = re.AsMemgrindNumber("bytes");}),
                new ParseSingleLineItem("==", "possibly lost: (?<bytes>[0-9,]+) bytes in (?<blocks>[0-9,]+) blocks", re => { result.PossiblyLostBlocks = re.AsMemgrindNumber("blocks"); result.PossiblyLostBytes = re.AsMemgrindNumber("bytes");}),
                new ParseSingleLineItem("==", "still reachable: (?<bytes>[0-9,]+) bytes in (?<blocks>[0-9,]+) blocks", re => { result.ReachableBlocks = re.AsMemgrindNumber("blocks"); result.ReachableBytes = re.AsMemgrindNumber("bytes");}),
                new ParseSingleLineItem("==", "suppressed: (?<bytes>[0-9,]+) bytes in (?<blocks>[0-9,]+) blocks", re => { result.SuppressedBlocks = re.AsMemgrindNumber("blocks"); result.SuppressedBytes = re.AsMemgrindNumber("bytes");}),
            };
            var parseItems = new List<ParseItemBase>()
            {
                summaryParseItems,
                new ParseMultilineMessage("== Conditional jump", "Conditional jump or move", result),
                new ParseMultilineMessage("== Invalid read of size", "Invalid Read", result),
                new ParseLossRecord("definitely lost", result.DefinitelyLost),
                new ParseLossRecord("indirectly lost", result.IndirectlyLost),
                new ParseLossRecord("possibly lost", result.PossiblyLost),
                new ParseLossRecord("still reachable", result.StillReachable),
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

                if (line.StartsWith("==16322== Memcheck, a memory error detector"))
                {
                    foreach (var p in parseItems)
                    {
                        p.Reset();
                    }
                }
            }

            return result;
        }
    }
}
