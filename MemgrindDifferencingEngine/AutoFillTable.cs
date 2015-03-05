
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MemgrindDifferencingEngine.Util;
using System;
using System.Collections.Generic;

namespace MemgrindDifferencingEngine
{
    /// <summary>
    /// A table that will auto-fill.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class AutoFillTable<T>
    {
        class RowAdder
        {
            public string Name { get; set; }
            public Func<T, int> Fetcher { get; set; }
        }

        /// <summary>
        /// A list of the by-row adders
        /// </summary>
        private List<RowAdder> _adders = new List<RowAdder>();

        /// <summary>
        /// Rows (then columns) of our table
        /// </summary>
        Dictionary<string, Dictionary<string, Cell>> _byRow = new Dictionary<string, Dictionary<string, Cell>>();

        /// <summary>
        /// We will be doing fill by row - this will add a row number automatically
        /// </summary>
        /// <param name="rowName"></param>
        /// <param name="fetcher"></param>
        internal void AddRowNumber(string rowName, Func<T, int> fetcher)
        {
            _adders.Add(new RowAdder() { Name = rowName, Fetcher = fetcher });
        }

        /// <summary>
        /// We have a column, fill it!
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="info"></param>
        internal void FillColumn(string colName, T info)
        {
            foreach (var row in _adders)
            {
                var c = row.Fetcher(info).AsCell();
                if (!_byRow.ContainsKey(row.Name))
                {
                    _byRow[row.Name] = new Dictionary<string, Cell>();
                }
                _byRow[row.Name][colName] = c;
            }
        }

        /// <summary>
        /// The hard part - insert everything into the worksheet.
        /// </summary>
        /// <param name="worksheetPart"></param>
        internal void DumpToExcel(WorksheetPart worksheetPart, SpreadsheetDocument doc)
        {
            var stp = doc.GetSharedStringPart();

            // Get a list of all column names
            var columnNames = new HashSet<string>();
            foreach (var r in _byRow)
            {
                columnNames.AddRange(r.Value.Keys);
            }

            // Now, do each column. The first col's header is blank.
            var startRow = (uint)2;
            var rowIndex = startRow + 1;
            foreach (var r in _byRow.Keys)
            {
                worksheetPart.SetCell("A", rowIndex, r.AsCell(stp));
                rowIndex++;
            }

            // Now do the column names
            var col = new ExcelColumn("B");
            foreach (var cname in columnNames)
            {
                worksheetPart.SetCell(col, startRow, cname.AsCell(stp));
                rowIndex = startRow + 1;
                foreach (var r in _byRow)
                {
                    if (r.Value.ContainsKey(cname))
                    {
                        worksheetPart.SetCell(col, rowIndex, r.Value[cname]);
                    }
                    rowIndex++;
                }
                col++;
            }
        }
    }
}
