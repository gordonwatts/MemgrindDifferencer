﻿
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using MemgrindDifferencingEngine.DataModel;
using MemgrindDifferencingEngine.ExcelHelpers;
using MemgrindDifferencingEngine.Util;
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
                results.Close();
            }
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
                var allKeys = infos.SelectMany(i => i.GrindDumpErrors).Select(i => i.Key).ToHashSet();

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
