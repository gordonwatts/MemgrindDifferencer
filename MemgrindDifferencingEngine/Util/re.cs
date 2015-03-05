using System.Text.RegularExpressions;

namespace MemgrindDifferencingEngine.Util
{
    static class re
    {
        /// <summary>
        /// Return a parsed number from a valgrind input regular expression parse.
        /// </summary>
        /// <param name="gp"></param>
        /// <returns></returns>
        public static int AsMemgrindNumber(this Group gp)
        {
            return int.Parse(gp.Value.Replace(",", ""));
        }

        /// <summary>
        /// Return a parsed number from a valgrind input regular expression group collection.
        /// </summary>
        /// <param name="gps"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static int AsMemgrindNumber(this Match match, string groupName)
        {
            return int.Parse(match.Groups[groupName].Value.Replace(",", ""));
        }
    }
}
