
using System.Collections.Generic;
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

        public MemgrindInfo()
        {
            GrindDumpErrors = new Dictionary<string, MemGrindDumpError>();
            DefinitelyLost = new Dictionary<string, MemGrindLossRecord>();
            PossiblyLost = new Dictionary<string, MemGrindLossRecord>();
            StillReachable = new Dictionary<string, MemGrindLossRecord>();
        }

    }
}
