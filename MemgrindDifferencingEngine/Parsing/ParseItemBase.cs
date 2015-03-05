
namespace MemgrindDifferencingEngine.Parsing
{
    /// <summary>
    /// base class for parsing items
    /// </summary>
    abstract class ParseItemBase
    {
        /// <summary>
        /// Prase the line.
        /// </summary>
        /// <param name="line"></param>
        public abstract void Process(string line);
    }
}
