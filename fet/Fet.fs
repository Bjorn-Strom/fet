namespace Fet

    [<RequireQualifiedAccess>]
    module Expect =
        let equal (actual: 'a) (expected: 'a): unit =
            if actual <> expected then
                failwith $"expected {actual} to be equal {expected}."

    module Test =
        type TestList = { Name: string; Tests: (string * (unit -> unit)) list }
        let test (name: string) (f: unit -> unit) = name, f
        let testList (name: string) testList: TestList = { Name = name; Tests = testList}

        let runTests (tests: TestList list) =
            let stats =
                tests
                |> List.map (fun testList ->
                    printfn "Test suite: %A" testList.Name
                    testList.Tests
                    |> Seq.fold (fun stats (k, v) ->
                        try
                            v ()
                            printfn $"\t\u2705  - {k}"
                            {| Succeeded = stats.Succeeded + 1; Failed = stats.Failed |}
                        with
                        | e ->
                            printfn $"\t\u274c  - Error in test \"{k}\" failed with: {e.Message}"
                            {| Succeeded = stats.Succeeded; Failed = stats.Failed + 1 |} ) {| Succeeded = 0; Failed = 0 |}
                    )
                |> List.reduce (fun acc x -> {| Succeeded = acc.Succeeded + x.Succeeded; Failed = acc.Failed + x.Failed |})

            printfn $"\nRan: {stats.Succeeded + stats.Failed}"
            printfn $"Succeeded: {stats.Succeeded}"
            printfn $"Failed: {stats.Failed}"