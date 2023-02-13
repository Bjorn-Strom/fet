namespace Fet

    [<RequireQualifiedAccess>]
    module Expect =
        let equal (actual: 'a) (expected: 'a): unit =
            if actual <> expected then
                failwith $"Expected: {expected}\nActual: {actual}"

    [<AutoOpen>]
    module Test =
        type Test = {
            Name: string
            Message: string option
        }
        type TestListResult = {
            ListName: string
            Succeeded: int
            Tests: Test list
        }
        type TestList = { Name: string; Tests: (string * (unit -> unit)) list }
        let test (name: string) (f: unit -> unit) = name, f
        let testList (name: string) testList: TestList = { Name = name; Tests = testList}

        let runTests (tests: TestList list) =
            let testResults =
                tests
                |> List.map (fun testList ->
                    //printfn "Test suite: %A" testList.Name
                    testList.Name, testList.Tests
                    |> Seq.map (fun (key, func)  ->
                        try
                            func ()
                            //printfn $"\t\u2705  - {k}"
                            { Name = key; Message = None }
                        with
                        | e ->
                            //printfn $"\t\u274c  - Error in test \"{k}\" failed with: {e.Message}"
                            { Name = key; Message = Some e.Message}
                    ))

            testResults
            |> List.iter (fun (testName, stats) -> 
                printfn $"Test list: {testName}"
                printfn "------------------------------"
                stats
                |> Seq.iter (fun (failed: Test) ->
                    if failed.Message.IsSome then 
                        printfn $"[FAIL] - {failed.Name}"
                        printfn "%A" failed.Message.Value)
                let total = Seq.length stats
                let succeeded =
                    stats
                    |> Seq.filter (fun (x: Test) -> x.Message.IsNone)
                    |> Seq.length
                let failed = total-succeeded;
                printfn $"Tests run: {total}, Passed: {succeeded}, Failed: {failed}"
                printfn "")

            let total = Seq.length testResults
            let succeeded =
                testResults
                |> Seq.map snd
                |> Seq.filter (fun (x: Test) -> x.Message.IsNone)
                |> Seq.length

            printfn "Total tests run: {}, Passed: {}, Failed: {}"

// TODO: Total count
// TODO: Send verbose in command line to see all
// TODO: XML thing?