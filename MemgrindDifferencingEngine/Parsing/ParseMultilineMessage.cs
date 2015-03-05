
using MemgrindDifferencingEngine.DataModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace MemgrindDifferencingEngine.Parsing
{
    /// <summary>
    /// Parse one of the typical multiline messages.
    /// </summary>
    class ParseMultilineMessage : ParseItemBase
    {
        private string _name;
        private string _containsText;
        bool _active = false;
        List<string> _currentError = new List<string>();
        private MemgrindInfo _info;
        public ParseMultilineMessage(string containsText, string name, MemgrindInfo info)
        {
            _name = name;
            _containsText = containsText;
            _info = info;
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
                    if (!_info.GrindDumpErrors.ContainsKey(key))
                    {
                        _info.GrindDumpErrors[key] = new MemGrindDumpError();
                        _info.GrindDumpErrors[key].Name = _name;
                    }
                    _info.GrindDumpErrors[key].Occurances++;

                    // Reset
                    _active = false;
                    _currentError.Clear();
                }
                else
                {
                    // In the middle of parsing this guy
                    _currentError.Add(line);
                }
            }
            else
            {
                if (line.StartsWith("==") && line.Contains(_containsText))
                {
                    _active = true;
                    _currentError.Add(line);
                }
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
