using MemgrindDifferencingEngine.DataModel;
using MemgrindDifferencingEngine.Util;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MemgrindDifferencingEngine.Parsing
{
    /// <summary>
    /// Parse a loss record
    // ==16280== 36 bytes in 3 blocks are definitely lost in loss record 96,253 of 173,622
    /// </summary>
    class ParseLossRecord : ParseMultilineMessageBase
    {
        private string _lossType;
        public ParseLossRecord(string lossType, Dictionary<string, MemGrindLossRecord> errors)
            : base(lossType + " in loss record")
        {
            _lossType = lossType;
            _errors = errors;
        }

        // Parse the loss record header
        private static Regex _lossParse = new Regex("(?<bytes>[0-9,]+) .*bytes in (?<blocks>[0-9,]+) blocks");
        // ==8704== 3,530 (72 direct, 3,458 indirect) bytes in 1 blocks are definitely lost in loss record 187,919 of 189,693
        // ==8704== 40 bytes in 1 blocks are definitely lost in loss record 108,789 of 189,693

        // Hold onto the results
        private Dictionary<string, MemGrindLossRecord> _errors;

        /// <summary>
        /// We found one. Parse and record it.
        /// </summary>
        /// <param name="fullLossMessage"></param>
        protected override void RecordError(string fullLossMessage)
        {
            var lineInfo = fullLossMessage.FirstLine();
            var key = fullLossMessage.AfterFirstLine();
            var m = _lossParse.Match(lineInfo);
            if (!m.Success)
            {
                throw new ArgumentException(string.Format("Could not parse loss line '{0}'", fullLossMessage));
            }


            if (_errors.ContainsKey(key))
            {
                _errors[key].BytesLost += m.AsMemgrindNumber("bytes");
                _errors[key].BlocksLost += m.AsMemgrindNumber("blocks");
            }
            else
            {
                var r = new MemGrindLossRecord() { BlocksLost = m.AsMemgrindNumber("blocks"), BytesLost = m.AsMemgrindNumber("bytes") };
                _errors[key] = r;
            }
            _errors[key].FirstLine.Add(lineInfo);

        }
    }
}
