
using System.Collections.Generic;
namespace MemgrindDifferencingEngine.DataModel
{
    public class MemGrindLossRecord
    {
        public int BytesLost { get; set; }
        public int BlocksLost { get; set; }

        public List<string> FirstLine { get; set; }

        public MemGrindLossRecord()
        {
            FirstLine = new List<string>();
        }

        /// <summary>
        /// Add the info from two of these together.
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public static MemGrindLossRecord operator +(MemGrindLossRecord o1, MemGrindLossRecord o2)
        {
            var r = new MemGrindLossRecord()
            {
                BytesLost = o1.BytesLost + o2.BytesLost,
                BlocksLost = o1.BlocksLost + o2.BlocksLost,
                FirstLine = new List<string>()
            };
            r.FirstLine.AddRange(o1.FirstLine);
            r.FirstLine.AddRange(o2.FirstLine);

            return r;
        }
    }
}
