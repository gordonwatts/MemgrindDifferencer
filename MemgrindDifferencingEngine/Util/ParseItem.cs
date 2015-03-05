using System;
using System.Text.RegularExpressions;

namespace MemgrindDifferencingEngine.Util
{
    /// <summary>
    /// Helper class to help with parsing various single line items
    /// </summary>
    class ParseItem
    {
        private string _startsWith;
        private Regex _re;
        private Action<Match> _actOnResult;
        public ParseItem(string startsWith, string regPattern, Action<Match> parseIt)
        {
            _startsWith = startsWith;
            _re = new Regex(regPattern);
            _actOnResult = parseIt;

        }

        /// <summary>
        /// Process this line.
        /// </summary>
        /// <param name="line"></param>
        internal void Process(string line)
        {
            if (line.StartsWith(_startsWith))
            {
                var m = _re.Match(line);
                if (m.Success)
                {
                    _actOnResult(m);
                }
            }
        }
    }
}
