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
                    testList.Name, testList.Tests
                    |> Seq.map (fun (key, func)  ->
                        try
                            func ()
                            { Name = key; Message = None }
                        with
                        | e ->
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
                        printfn "%s" failed.Message.Value)
                let total = Seq.length stats
                let succeeded =
                    stats
                    |> Seq.filter (fun (x: Test) -> x.Message.IsNone)
                    |> Seq.length
                let failed = total-succeeded;
                printfn $"Tests run: {total}, Passed: {succeeded}, Failed: {failed}"
                printfn "")

            let total =
                testResults
                |> Seq.collect snd
                |> Seq.length
            let succeeded =
                testResults
                |> Seq.collect snd
                |> Seq.filter (fun x -> x.Message.IsNone)
                |> Seq.length
            let failed = total - succeeded

            printfn $"Total tests run: {total}, Passed: {succeeded}, Failed: {failed}"
            failed