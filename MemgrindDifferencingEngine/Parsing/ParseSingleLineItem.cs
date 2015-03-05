using System;
using System.Text.RegularExpressions;

namespace MemgrindDifferencingEngine.Parsing
{
    /// <summary>
    /// Helper class to help with parsing various single line items
    /// </summary>
    class ParseSingleLineItem : ParseItemBase
    {
        private string _containsText;
        private Regex _re;
        private Action<Match> _actOnResult;
        public ParseSingleLineItem(string containsText, string regPattern, Action<Match> parseIt)
        {
            _containsText = containsText;
            _re = new Regex(regPattern);
            _actOnResult = parseIt;

        }

        /// <summary>
        /// Process this line.
        /// </summary>
        /// <param name="line"></param>
        public override void Process(string line)
        {
            if (line.StartsWith("==") && line.Contains(_containsText))
            {
                var m = _re.Match(line);
                if (m.Success)
                {
                    _actOnResult(m);
                }
            }
        }

        /// <summary>
        /// We do nothing on reset.
        /// </summary>
        public override void Reset()
        {
        }
    }
}
