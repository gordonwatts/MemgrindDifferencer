
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MemgrindDifferencingEngine.DataModel;
using System.IO;
namespace MemgrindDifferencingEngine
{
    /// <summary>
    /// Dump the results of several files to excel
    /// </summary>
    public static class DumpToExcel
    {
        /// <summary>
        /// Dump the info to an excel file for "easy" viewing.
        /// </summary>
        /// <param name="outputFile"></param>
        /// <param name="info"></param>
        public static void Dump(FileInfo outputFile, MemgrindInfo[] info)
        {
            using (var results = SpreadsheetDocument.Create(outputFile.FullName, SpreadsheetDocumentType.Workbook))
            {
                DumpSummary(results, info);
                results.Close();
            }
        }

        /// <summary>
        /// Dump a summary sheet
        /// </summary>
        /// <param name="spreadsheetDocument"></param>
        private static void DumpSummary(SpreadsheetDocument spreadsheetDocument, MemgrindInfo[] infos)
        {
            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Summary"
            };
            sheets.Append(sheet);

            // Create a table with summary data in it
            var t = new AutoFillTable<MemgrindInfo>();
            t.AddRowNumber("Lost Blocks", tinfo => tinfo.LostBlocks);
            t.AddRowNumber("Lost Bytes", tinfo => tinfo.LostBytes);
            t.AddRowNumber("Possibly Lost Blocks", tinfo => tinfo.PossiblyLostBlocks);
            t.AddRowNumber("Possibly Lost Bytes", tinfo => tinfo.PossiblyLostBytes);
            t.AddRowNumber("Indirectly Lost Blocks", tinfo => tinfo.IndirectlyLostBlocks);
            t.AddRowNumber("Indirectly Lost Bytes", tinfo => tinfo.IndirectlyLostBytes);
            t.AddRowNumber("Supressed Blocks", tinfo => tinfo.SuppressedBlocks);
            t.AddRowNumber("Supressed Bytes", tinfo => tinfo.SuppressedBytes);

            foreach (var info in infos)
            {
                t.FillColumn(info.Description, info);
            }

            t.DumpToExcel(worksheetPart, spreadsheetDocument);

            worksheetPart.Worksheet.Save();


            workbookpart.Workbook.Save();
        }
    }
}
