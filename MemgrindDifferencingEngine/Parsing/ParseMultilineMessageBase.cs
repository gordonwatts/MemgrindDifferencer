
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace MemgrindDifferencingEngine.Parsing
{
    /// <summary>
    /// Parse one of the typical multiline messages.
    /// </summary>
    abstract class ParseMultilineMessageBase : ParseItemBase
    {
        private string _containsText;
        bool _active = false;
        List<string> _currentError = new List<string>();

        public ParseMultilineMessageBase(string containsText)
        {
            _containsText = containsText;
        }

        private static Regex endOfError = new Regex("^==[0-9]+==$");

        /// <summary>
        /// We do multiple lines, so we have to see when we are active (and when we aren't!).
        /// </summary>
        /// <param name="line"></param>
        public override void Process(string line)
        {
            if (_active)
            {
                // Is this the last line of this error?
                if (endOfError.Match(line.Trim()).Success)
                {
                    // Record, and store for next time.
                    var bld = new StringBuilder();
                    foreach (var keyLine in _currentError)
                    {
                        bld.AppendLine(keyLine);
                    }
                    var key = bld.ToString();
                    RecordError(key);

                    // Reset
                    _active = false;
                    _currentError.Clear();
                }
                else
                {
                    // In the middle of parsing this guy
                    _currentError.Add(StripLineOfValgrind(line));
                }
            }
            else
            {
                if (line.StartsWith("==") && line.Contains(_containsText))
                {
                    _active = true;
                    _currentError.Add(StripLineOfValgrind(line));
                }
            }
        }

        protected abstract void RecordError(string key);

        private static Regex goodLinePart = new Regex("^==[0-9]+== (?<msg>.+)$");
        private static Regex findHex = new Regex("0x[0-9A-Fa-f]+");

        /// <summary>
        /// Clean up the line of valgrind message. In particular, remove some of the hex info
        /// that is bound to be different run-to-run.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static string StripLineOfValgrind(string line)
        {
            var newline = findHex.Replace(line, "0x0");
            var m = goodLinePart.Match(newline);
            if (!m.Success)
            {
                return newline;
            }
            else
            {
                return m.Groups["msg"].Value;
            }
        }

        /// <summary>
        /// We do nothing when we reset.
        /// </summary>
        public override void Reset()
        {
        }
    }
}
