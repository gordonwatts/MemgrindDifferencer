using System;

namespace MemgrindDifferencingEngine.Util
{
    static class Strings
    {
        /// <summary>
        /// Return the first line of a string.
        /// </summary>
        /// <param name="multiline"></param>
        /// <returns></returns>
        public static string FirstLine(this string multiline)
        {
            return multiline.Substring(0, multiline.IndexOf(Environment.NewLine));
        }
    }
}
