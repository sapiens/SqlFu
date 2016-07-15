 // include Fake lib
#r "tools/FAKE/tools/FakeLib.dll"

#load "settings.fsx"
#load "utils.fsx"

open Fake
open StringHelper
open EnvironmentHelper
open Settings
open Utils
open FileSystemHelper



let buildDir = "./build/"



let pkgFiles()=
    let packFilesPattern=outDir @@ "*.nupkg"
    let ignoreSymbolsPattern=outDir @@ "*symbols.nupkg"
    (--) !!packFilesPattern <| ignoreSymbolsPattern |> Seq.head





Target "Clean" (fun _ ->  
   CleanDir outDir   
)
 
Target "Build" (fun _ -> 
    restore projDir |> checkResult "Restore failed"
    compile projDir |> checkResult "Build failed"     
)
 
Target "Pack" ( fun _ ->
    pack projDir |> ignore
    additionalPack 
        |> Seq.map (fun d -> (projDir+d))
        |> Seq.iter (fun d-> 
                            trace ("packing "+d)
                            pack d |> ignore)    
)

Target "Test" (fun _ ->
   runTests testDir |> checkResult "Tests failed"  
   runTests ("..\src\Tests.SqlServer") |> checkResult "Tests failed"  
)


Target "Push"(fun _ -> pkgFiles()|> push |> ignore)

Target "Local"( fun _ ->
   !! pkgFiles() |> CopyFiles localNugetRepo)



// Dependencies

"Clean"
    ==>"Build"
    ==>"Test"
    ==>"Pack"
    ==>"Local"

"Clean"
    ==>"Build"
    ==>"Test"
    ==>"Pack"
    ==>"Push"
   

 
// start build
RunTargetOrDefault "Test"