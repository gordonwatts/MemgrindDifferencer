
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

        /// <summary>
        /// We've seen the start of a new run in a multi-run file.
        /// </summary>
        public abstract void Reset();
    }
}
