// Simple program that will extract the routines from a valgrind's memcheck log messages.
// It then tabulates the size of each leak and routine, and dumps them in a format that
// excel can use.
//
// In excel: create a table of everything, make a pivot table from that table. Use the routine
// name as the row, and then drag the bytes and blocks into the Totals box. Then you can right click on
// one of the names and stort by blocks or bytes. You can select a name and hide it to get rid of the obvious
// ones (like TNew::New for example).

// Use this or something similar to open the log file parser engine if you are using the command line.
// #r @"C:\Users\Gordon\Documents\Code\MemgrindDifferencer\MemgrindDifferencingEngine\bin\Debug\MemgrindDifferencingEngine.dll";;
open MemgrindDifferencingEngine
open System.Collections.Generic
open MemgrindDifferencingEngine.DataModel
open System.Text.RegularExpressions

type LossInfo =
    | Routine of name:string * bytes:int * blocks:int

let printInCSVFormat li =
    match li with
    | Routine (name, bytes, block) -> printfn "\"%s\"\t%d\t%d" name bytes block 

// This code parses a key/value pair from a loss record and turns it into a list of LostInfo
// objects. We remove all duplicate routines, and attach the loss records into to
// each object. We are using RE's (hence the wierd map'ing that we end up doing here).
let namefinder = Regex(@": ([^\(]+)\(")
let lossRecordAsRoutines (lr : KeyValuePair<string,MemGrindLossRecord>) =
    let k = lr.Key
    let lrinfo = lr.Value
    namefinder.Matches k 
    |> Seq.cast
    |> Seq.map (fun (x : Match) -> x.Groups.[1].Value)
    |> Seq.distinct
    |> Seq.map (fun x -> Routine (x, lrinfo.BytesLost, lrinfo.BlocksLost))

// Arg1 is the file.
[<EntryPoint>]
let main argv = 
    let o = MemgrindDifferencingEngine.MemgrindLogParser.Parse argv.[0]
    printfn "Routine\tLost Bytes\tLost Blocks"
    let allR = o.DefinitelyLost |> Seq.map lossRecordAsRoutines |> Seq.concat |> Seq.iter printInCSVFormat

    0 // return an integer exit code
