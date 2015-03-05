
using System;
namespace MemgrindDifferencingEngine.ExcelHelpers
{
    class ExcelColumn
    {
        private string _col;
        public ExcelColumn(string colName)
        {
            if (colName.Length != 1)
                throw new ArgumentException("Can't have col length greater than 1");

            _col = colName;
        }

        /// <summary>
        /// Increment by one column
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ExcelColumn operator ++(ExcelColumn t)
        {
            t._col = string.Format("{0}", (char)((int)t._col[0] + 1));
            return t;
        }

        public string Name { get { return _col; } }
    }
}
