using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Linq;

namespace MemgrindDifferencingEngine.ExcelHelpers
{
    static class ExcelUtils
    {
        /// <summary>
        /// Return an integer as a cell
        /// </summary>
        public static Cell AsCell(this int value)
        {
            var c = new Cell();
            c.CellValue = new CellValue(value.ToString());
            c.DataType = new EnumValue<CellValues>(CellValues.Number);
            return c;
        }

        /// <summary>
        /// Return the shared string part for the worksheet, creating it if it doesn't already exist.
        /// </summary>
        /// <param name="ws"></param>
        /// <returns></returns>
        public static SharedStringTablePart GetSharedStringPart(this SpreadsheetDocument excel)
        {
            var s = excel.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            if (s != null)
                return s;
            return excel.WorkbookPart.AddNewPart<SharedStringTablePart>();
        }

        /// <summary>
        /// Convert a string to a shared string cell.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Cell AsCell(this string value, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int index = 0;
            bool found = false;
            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == value)
                {
                    found = true;
                    break;
                }
                index++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            if (!found)
            {
                shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(value)));
                shareStringPart.SharedStringTable.Save();
            }

            // Now we can set the cell
            var c = new Cell();
            c.CellValue = new CellValue(index.ToString());
            c.DataType = new EnumValue<CellValues>(CellValues.SharedString);
            return c;
        }

        /// <summary>
        /// Set the cell to something, if it si there, replace it, otherwise create it.
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="colName"></param>
        /// <param name="row"></param>
        /// <param name="content"></param>
        public static void SetCell(this WorksheetPart ws, string colName, uint rowIndex, Cell content)
        {
            var worksheet = ws.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = colName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).FirstOrDefault();
            if (row == null)
            {
                row = new Row() { RowIndex = rowIndex };
                var beforeRow = sheetData.Elements<Row>().Where(r => r.RowIndex > rowIndex).FirstOrDefault();
                sheetData.InsertBefore(row, beforeRow);
            }

            // If there is not a cell with the specified column name, insert one.  
            var alreadyThereCell = row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).FirstOrDefault();
            if (alreadyThereCell != null)
            {
                throw new InvalidOperationException(string.Format("Cell already exists: {0}", cellReference));
            }
            // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
            Cell refCell = row.Elements<Cell>().Where(rc => string.Compare(rc.CellReference.Value, cellReference, true) > 0).FirstOrDefault();
            content.CellReference = cellReference;
            row.InsertBefore(content, refCell);
        }

        /// <summary>
        /// Set the cell given a column name object
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="colName"></param>
        /// <param name="row"></param>
        /// <param name="content"></param>
        public static void SetCell(this WorksheetPart ws, ExcelColumn colName, uint row, Cell content)
        {
            SetCell(ws, colName.Name, row, content);
        }

        /// <summary>
        /// Track what sheet id we should use next.
        /// </summary>
        private static uint _gNextFreeSheetID = 1;

        /// <summary>
        /// Create a new worksheet part
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static WorksheetPart CreateSheet(this SpreadsheetDocument doc, string name)
        {
            // Add a WorkbookPart to the document.
            if (doc.WorkbookPart == null)
            {
                doc.AddWorkbookPart().Workbook = new Workbook();
            }
            var workbookpart = doc.WorkbookPart;

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add or get the sheets from the workbook.
            var sheets = workbookpart.Workbook.GetFirstChild<Sheets>();
            if (sheets == null)
            {
                sheets = doc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            }

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = doc.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = _gNextFreeSheetID,
                Name = name
            };
            _gNextFreeSheetID++;
            sheets.Append(sheet);

            return worksheetPart;
        }
    }
}
