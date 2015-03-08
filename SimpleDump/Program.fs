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

// Flags:
// --contains <routine-name> the stack must contian a routine somewhere in there
// --dumpDefinitely, --dumpPossibly, --dumpIndirectly

open MemgrindDifferencingEngine
open System.Collections.Generic
open MemgrindDifferencingEngine.DataModel
open System.Text.RegularExpressions
open Microsoft.FSharp.Text

let printInCSVFormat li =
    let (name, bytes, block) = li
    printfn "\"%s\"\t%d\t%d" name bytes block 

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
    |> Seq.map (fun x -> (x, lrinfo.BytesLost, lrinfo.BlocksLost))

// Get all loss records from a file (all the different types)
let lossRecordFiles (fname : string) =
    MemgrindDifferencingEngine.MemgrindLogParser.Parse fname

// Given a loss record, and what we should look at, turn it into a stream
// of loss records.
let lossRecordFileAsKVPairs (fullFileLR : MemgrindInfo) (dumpDefinitely : bool) (dumpIndirectly : bool) (dumpPossibly : bool) =
    let emptyDict = new Dictionary<string,MemGrindLossRecord> ()
    let makeDict flag dict =
        match flag with
        | true -> dict 
        | false -> emptyDict
    let definatly = makeDict dumpDefinitely fullFileLR.DefinitelyLost
    let possibly = makeDict dumpPossibly fullFileLR.PossiblyLost
    let indirectly = makeDict dumpIndirectly fullFileLR.IndirectlyLost
    [definatly; possibly; indirectly] |> Seq.ofList |> Seq.concat 


// Build a filter out of a function that takes the parts of a routine. This just makes life simpler
// by splitting the tuple into individual items and calling the function.
let buildRoutineNameFilter ffunc =
    let myf (x: string*int*int) =
        let (name, bytes, blocks) = x
        ffunc name bytes blocks
    myf

// Very simple adder.
let addRoutineNames (left:int*int) (right:int*int) =
    let (l2, l3) = left
    let (r2, r3) = right
    (l2 + r2, l3 + r3)

// Create a function that will return a sequence of "reduced" routine tuples, given
// an addr function. It will be called with two operands where the routine name is the same.
let reduceLR addr sequence =
    Seq.fold 
        (fun (state: Map<string, int*int>) (routine, (bytes:int), (blocks:int)) ->
            let origInfo = match Map.containsKey routine state with
                            | true -> state.[routine]
                            | false -> (0, 0)   
            Map.add routine (addr origInfo (bytes, blocks)) state)
        Map.empty
        sequence
    |> Seq.map 
        (fun x -> 
            let (bytes, blocks) = x.Value
            (x.Key, bytes, blocks))

// Arguments that have no flag are file names. We do one at a time. Not suggested as the output will be a bit confused.
[<EntryPoint>]
let main argv = 
    // Argument Parsing, which will drive this hole thing. 
    let mustContainText = ref ""
    let dumpDefinitely = ref false
    let dumpPossibly = ref false
    let dumpIndirectly = ref false
    let specs =
        ["--contains", ArgType.String (fun s -> mustContainText := s), "A method that the stack must contain to be included"
         "--dumpDefinitely", ArgType.Set dumpDefinitely, "Dump definately lost blocks"
         "--dumpPossibly", ArgType.Set dumpPossibly, "Dump possibly lost blocks"
         "--dumpIndirectly", ArgType.Set dumpIndirectly, "Dump indirectly lost blocks"
        ]
        |> List.map (fun (sh, ty, desc) -> ArgInfo(sh, ty, desc)) 

    // The primary function to do the work. Called once per file.
    let compile (s:string) =
        let o = MemgrindDifferencingEngine.MemgrindLogParser.Parse s
        let filter lst = 
            match mustContainText.Value.Length = 0 with
            | true -> true
            | false -> Seq.exists (buildRoutineNameFilter (fun routine bytes blocks -> routine.Contains(mustContainText.Value))) lst

        let emptyDict = new Dictionary<string,MemGrindLossRecord> ()
        let makeDict (flag:bool ref) dict =
            match flag.Value with
            | true -> dict 
            | false -> emptyDict

        let definatly = makeDict dumpDefinitely o.DefinitelyLost
        let possibly = makeDict dumpPossibly o.PossiblyLost
        let indirectly = makeDict dumpIndirectly o.IndirectlyLost

        let allItems = [definatly; possibly; indirectly] |> Seq.ofList |> Seq.concat |> Seq.map lossRecordAsRoutines |> Seq.where filter |> Seq.concat |> reduceLR addRoutineNames |> Seq.iter printInCSVFormat

        ()

    printfn "Routine\tLost Bytes\tLost Blocks"
    let () =
        ArgParser.Parse(specs, compile)   

    // Parse the file.
    //let o = MemgrindDifferencingEngine.MemgrindLogParser.Parse argv.[0]
    //printfn "Routine\tLost Bytes\tLost Blocks"
    //let filter lst = Seq.exists (fun (x:LossInfo) -> match x with | Routine (name, bytes, blocks) -> name.Contains("htlExecute")) lst
    //let allR = o.DefinitelyLost |> Seq.map lossRecordAsRoutines |> Seq.where filter |> Seq.concat |> Seq.iter printInCSVFormat

    0 // return an integer exit code
