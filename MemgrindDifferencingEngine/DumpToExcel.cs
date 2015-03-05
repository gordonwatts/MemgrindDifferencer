
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using MemgrindDifferencingEngine.DataModel;
using MemgrindDifferencingEngine.ExcelHelpers;
using MemgrindDifferencingEngine.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                DumpGrindErrors(results, info);
                DumpGrindLossBlocks(results, info, "Definitely Lost", t => t.DefinitelyLost);
                DumpGrindLossBlocks(results, info, "Possibly Lost", t => t.PossiblyLost);
                DumpGrindLossBlocks(results, info, "Still Reachable", t => t.StillReachable);
                results.Close();
            }
        }

        /// <summary>
        /// Dump a set of loss records.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="info"></param>
        /// <param name="sheetName"></param>
        /// <param name="extractLossRecord"></param>
        private static void DumpGrindLossBlocks(SpreadsheetDocument doc, MemgrindInfo[] infos, string sheetName, Func<MemgrindInfo, Dictionary<string, MemGrindLossRecord>> extractLossRecord)
        {
            var allKeys = infos.Select(t => extractLossRecord(t)).SelectMany(t => t.Keys).ToHashSet();

            var table = new AutoFillTable<Dictionary<string, MemGrindLossRecord>>();
            foreach (var r in allKeys)
            {
                table.AddRowNumber(r, tinfo => tinfo.ContainsKey(r) ? tinfo[r].BytesLost : 0);
            }

            foreach (var info in infos)
            {
                table.FillColumn(info.Description, extractLossRecord(info));
            }

            var wsp = doc.CreateSheet(sheetName);
            table.DumpToExcel(wsp, doc);
            doc.GetSharedStringPart().SharedStringTable.Save();
            wsp.Worksheet.Save();
        }

        /// <summary>
        /// Dump the memory errors that were found
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="infos"></param>
        private static void DumpGrindErrors(SpreadsheetDocument doc, MemgrindInfo[] infos)
        {
            var errorTypes = infos.SelectMany(i => i.GrindDumpErrors).Select(i => i.Value.Name).ToHashSet();

            foreach (var errorType in errorTypes)
            {
                var wsp = doc.CreateSheet(errorType);
                var allKeys = infos.SelectMany(i => i.GrindDumpErrors).Where(i => i.Value.Name == errorType).Select(i => i.Key).ToHashSet();

                var t = new AutoFillTable<Dictionary<string, MemGrindDumpError>>();
                foreach (var k in allKeys)
                {
                    t.AddRowNumber(k, tinfo => tinfo.Where(v => v.Key == k).Select(v => v.Value.Occurances).FirstOrDefault());
                }

                foreach (var info in infos)
                {
                    t.FillColumn(info.Description, info.GrindDumpErrors);
                }

                t.DumpToExcel(wsp, doc);
                wsp.Worksheet.Save();
            }
        }

        /// <summary>
        /// Dump a summary sheet
        /// </summary>
        /// <param name="spreadsheetDocument"></param>
        private static void DumpSummary(SpreadsheetDocument spreadsheetDocument, MemgrindInfo[] infos)
        {
            var worksheetPart = spreadsheetDocument.CreateSheet("Summary");

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
        }
    }
}
