namespace Example

    module Fable=
        open Fet

        let tests =
            let add a b = a + b

            testList "A list of tests" [
                test "Strings are equal" <| fun () ->
                    let helloWorld = "Hello World"
                    Expect.equal "Hello World" helloWorld

                test "Add 2 and 2 equals 4" <| fun () ->
                      let expected = 4
                      Expect.equal (add 2 2) expected

                test "This test should fail" <| fun () ->
                    Expect.equal (3+3) 5

                test "Length of empty list should be 0" <| fun () ->
                    Expect.equal (List.length []) 0
                ]

        runTests [ tests ]