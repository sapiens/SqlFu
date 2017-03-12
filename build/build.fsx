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





Target "Clean" (fun _ ->  
   CleanDir outDir   
)
 
Target "Build" (fun _ -> 
    restore projDir |> ignore
    let result= compile projDir
    if result <> 0 then failwith "build failed"            
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
   runTests testDir
  
)

let pkgFiles= lazy(
    let packFilesPattern=outDir @@ "*.nupkg"    
    let ignoreSymbolsPattern=outDir @@ "*symbols.nupkg"
    let files=ignoreSymbolsPattern|> (--) !!packFilesPattern
    files
    )

Target "Push"(fun _ -> pkgFiles.Value |> Seq.iter(fun i-> push i |> ignore)  )

Target "Local"( fun _ ->
     pkgFiles.Value |> CopyFiles localNugetRepo
)

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