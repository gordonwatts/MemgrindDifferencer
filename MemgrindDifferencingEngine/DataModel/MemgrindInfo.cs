
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
    }
}
