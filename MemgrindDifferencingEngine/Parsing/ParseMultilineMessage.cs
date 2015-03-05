
using MemgrindDifferencingEngine.DataModel;
namespace MemgrindDifferencingEngine.Parsing
{
    /// <summary>
    /// Track number of times a multi-line message occurs
    /// </summary>
    class ParseMultilineMessage : ParseMultilineMessageBase
    {
        private MemgrindInfo _info;
        private string _name;
        /// <summary>
        /// Init for multi line counting
        /// </summary>
        /// <param name="containsText"></param>
        /// <param name="name"></param>
        /// <param name="info"></param>
        public ParseMultilineMessage(string containsText, string name, MemgrindInfo info)
            : base(containsText)
        {
            _info = info;
            _name = name;
        }

        /// <summary>
        /// Error has shown up - record it.
        /// </summary>
        /// <param name="key"></param>
        protected override void RecordError(string key)
        {
            if (!_info.GrindDumpErrors.ContainsKey(key))
            {
                _info.GrindDumpErrors[key] = new MemGrindDumpError();
                _info.GrindDumpErrors[key].Name = _name;
            }
            _info.GrindDumpErrors[key].Occurances++;
        }
    }
}
