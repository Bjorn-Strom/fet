open Fet

let tests =
        test.testList ""
            [ test.test "" <| fun () -> expect.equal 1 1
            ]

test.runAllTests [ tests ]