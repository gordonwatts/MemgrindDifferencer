
namespace MemgrindDifferencingEngine.Parsing
{
    /// <summary>
    /// Parse a loss record
    // ==16280== 36 bytes in 3 blocks are definitely lost in loss record 96,253 of 173,622
    /// </summary>
    class ParseLossRecord : ParseMultilineMessageBase
    {
        private string _lossType;
        public ParseLossRecord(string lossType)
            : base(lossType + " in loss record")
        {
            _lossType = lossType;
        }

        /// <summary>
        /// We found one. Parse and record it.
        /// </summary>
        /// <param name="key"></param>
        protected override void RecordError(string key)
        {
        }
    }
}
