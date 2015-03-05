
using MemgrindDifferencingEngine.DataModel;
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

            return result;
        }
    }
}
