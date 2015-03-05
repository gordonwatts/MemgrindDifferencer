
using MemgrindDifferencingEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MemgrindDifferencingEngine.DataModel
{
    /// <summary>
    /// Master class that contains all memgrind info
    /// </summary>
    public class MemgrindInfo
    {
        /// <summary>
        /// Get/Set the description of the file
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Lost bytes pulled from the summary
        /// </summary>
        public int LostBytes { get; set; }
        /// <summary>
        /// Lost blocks pulled from teh summary
        /// </summary>
        public int LostBlocks { get; set; }

        public int IndirectlyLostBlocks { get; set; }

        public int IndirectlyLostBytes { get; set; }

        public int PossiblyLostBlocks { get; set; }

        public int PossiblyLostBytes { get; set; }

        public int ReachableBlocks { get; set; }

        public int ReachableBytes { get; set; }

        public int SuppressedBlocks { get; set; }

        public int SuppressedBytes { get; set; }

        /// <summary>
        /// Get the list of errors we've seen so far
        /// </summary>
        public Dictionary<string, MemGrindDumpError> GrindDumpErrors { get; private set; }
        public Dictionary<string, MemGrindLossRecord> DefinitelyLost { get; private set; }
        public Dictionary<string, MemGrindLossRecord> PossiblyLost { get; set; }
        public Dictionary<string, MemGrindLossRecord> StillReachable { get; set; }
        public Dictionary<string, MemGrindLossRecord> IndirectlyLost { get; set; }

        public MemgrindInfo()
        {
            GrindDumpErrors = new Dictionary<string, MemGrindDumpError>();
            DefinitelyLost = new Dictionary<string, MemGrindLossRecord>();
            PossiblyLost = new Dictionary<string, MemGrindLossRecord>();
            StillReachable = new Dictionary<string, MemGrindLossRecord>();
            IndirectlyLost = new Dictionary<string, MemGrindLossRecord>();
        }

        /// <summary>
        /// Add the two sets together.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static Dictionary<string, MemGrindLossRecord> AddThem(Dictionary<string, MemGrindLossRecord> s1, Dictionary<string, MemGrindLossRecord> s2)
        {
            var allKeys = s1.Keys.Concat(s2.Keys).ToHashSet();

            var r = new Dictionary<string, MemGrindLossRecord>();
            var pairs = allKeys.Select(k => Tuple.Create<string, MemGrindLossRecord, MemGrindLossRecord>(k, s1.ContainsKey(k) ? s1[k] : null, s2.ContainsKey(k) ? s2[k] : null));
            foreach (var k in pairs)
            {
                if (k.Item2 == null)
                {
                    r[k.Item1] = k.Item3;
                }
                else if (k.Item3 == null)
                {
                    r[k.Item1] = k.Item2;
                }
                else
                {
                    r[k.Item1] = k.Item2 + k.Item3;
                }

            }

            return r;
        }
    }
}
