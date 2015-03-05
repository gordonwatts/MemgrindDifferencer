using System.Collections.Generic;
using System.IO;

namespace MemgrindDifferencingEngine.Parsing
{
    static class Files
    {
        /// <summary>
        /// Will read a file, one line at a time. Minimal buffering.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        public static IEnumerable<string> FileIterator(this FileInfo inputFile)
        {
            using (var stream = inputFile.OpenRead())
            {
                using (var readr = new StreamReader(stream))
                {
                    while (true)
                    {
                        var line = readr.ReadLine();
                        if (line == null)
                            break;
                        yield return line;
                    }
                }
            }
        }
    }
}
