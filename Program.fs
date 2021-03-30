type TestList = { Name: string; Tests: (string * (unit -> unit)) list }

[<RequireQualifiedAccess>]
module expect =
    let equal (actual: 'a) (expected: 'a): unit =
        if actual <> expected then
            failwith $"expected {actual} to be equal {expected}."

let test (name: string) (f: unit -> unit) = name, f
let testList (name: string) testList: TestList = { Name = name; Tests = testList}

let runAllTests (tests: TestList list) =
    let stopWatch = System.Diagnostics.Stopwatch.StartNew()
    let stats =
        tests
        |> List.map (fun testList ->
            printfn "Test suite: %A" testList.Name
            testList.Tests
            |> Seq.fold (fun stats (k, v) ->
                try
                    let stopWatch = System.Diagnostics.Stopwatch.StartNew()
                    v ()
                    stopWatch.Stop()
                    printfn $"\t\u2705  - {k} in {stopWatch.Elapsed.TotalMilliseconds} ms"
                    {| Succeeded = stats.Succeeded + 1; Failed = stats.Failed |}
                with
                | e ->
                    printfn $"\t\u274c  - Error in test \"{k}\" failed with: {e.Message}"
                    {| Succeeded = stats.Succeeded; Failed = stats.Failed + 1 |} ) {| Succeeded = 0; Failed = 0 |}
            )
        |> List.reduce (fun acc x -> {| Succeeded = acc.Succeeded + x.Succeeded; Failed = acc.Failed + x.Failed |})
    stopWatch.Stop()

    printfn $"\nRan {stats.Succeeded + stats.Failed} tests in {stopWatch.Elapsed.TotalMilliseconds} ms"
    printfn $"Succeeded: {stats.Succeeded}"
    printfn $"Failed: {stats.Failed}"

let foo =
    testList "foo"
            [ test "1 + 2" <| fun () -> (expect.equal 3 2)
              test "2 + 3" <| fun () -> (expect.equal 5 5)
            ]

let bar =
    testList "Bar"
        [ test "name is bjørn" <| fun () -> (expect.equal "bjørn" "bjørn")
          test "1 = 1" <| fun () -> (expect.equal 1 1)
        ]

let tests=
    [ foo
      bar
    ]

runAllTests tests
