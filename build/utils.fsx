#r "tools/FAKE/tools/FakeLib.dll"
#r "tools/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

#load "settings.fsx"

open Newtonsoft.Json
open System 
open Fake
open Settings


let outDir="artifacts" |> combinePaths currentDirectory 


type gjVersion={version:string}    
type globalJson={sdk:gjVersion}

let userPath=environVar "UserProfile"
let dnxVersion= 
    let file=ReadFileAsString (combinePaths projDir "global.json")
    let json:globalJson= JsonConvert.DeserializeObject<globalJson>(file)
    json.sdk.version    

let clr = combinePaths userPath ".dnx/runtimes/dnx-clr-win-x86."+dnxVersion+"/bin"
let clrCore = combinePaths userPath ".dnx/runtimes/dnx-coreclr-win-x86."+dnxVersion+"/bin"


let dnu = combinePaths clr "dnu.cmd"
let dnx runtime= runtime @@ "dnx.exe"

let runResult value msg = if value <> 0 then failwith msg

let restore (proj:string)= 
        let result = ExecProcessAndReturnMessages(fun c -> 
                                        c.FileName<-dnu
                                        c.Arguments<- String.Format("restore \"{0}\"",proj)
                                        )
                                        (TimeSpan.FromMinutes 5.0)  
      
        result.ExitCode

let compile proj= ExecProcess(fun c -> 
                                        c.FileName<-dnu
                                        c.Arguments<-("build "+proj+" --quiet --configuration release --out "+outDir))(TimeSpan.FromMinutes 5.0)

let runTests runtime dir= 
    let result = ExecProcess(fun c -> 
                                        c.FileName<- (dnx runtime)
                                        c.Arguments<-("-p \""+dir+"\" test"))(TimeSpan.FromMinutes 5.0)
    if result <> 0 then failwith "Testing failed"


let pack proj =ExecProcess(fun c -> 
                                        c.FileName<-dnu
                                        c.Arguments<-("pack "+proj+" --quiet --configuration release  --out "+outDir))(TimeSpan.FromMinutes 5.0)
let push files = ExecProcess(fun c ->
                            c.FileName<- (currentDirectory @@ nugetExeDir @@ "nuget.exe")
                            c.Arguments <- ("push "+ files))(TimeSpan.FromMinutes 5.0)
                                                        
