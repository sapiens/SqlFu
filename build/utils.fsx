#r "tools/FAKE/tools/FakeLib.dll"

#load "settings.fsx"

open System 
open Fake
open Settings


let localNugetRepo="E:/Libs/nuget"
let dotnet="dotnet.exe"
let nugetServer= "https://www.nuget.org/api/v2/package"

//relative to script
let nugetExeDir="tools"
let outDir=(projDir @@ "bin") @@ "Release"


let checkResult (msg:string) (res:int) = if res <> 0 then failwith msg

let restore (proj:string)= 
        let result = ExecProcessAndReturnMessages(fun c -> 
                                        c.FileName<-dotnet
                                        c.Arguments<- String.Format("restore \"{0}\"",proj)
                                        )
                                        (TimeSpan.FromMinutes 5.0)  
      
        result.ExitCode

let compile proj= ExecProcess(fun c -> 
                                        c.FileName<-dotnet
                                        c.Arguments<-("build "+proj+" -c release"))(TimeSpan.FromMinutes 5.0)


let runTests dir= ExecProcess(fun c -> 
                                        c.FileName<- dotnet
                                        c.Arguments<-("test  \""+dir+"\""))(TimeSpan.FromMinutes 5.0)
   
let pack proj =ExecProcess(fun c -> 
                                        c.FileName<-dotnet
                                        c.Arguments<-("pack "+proj+" --no-build -c Release"))(TimeSpan.FromMinutes 5.0)
let push file = ExecProcess(fun c ->
                            c.FileName<- (currentDirectory @@ nugetExeDir @@ "nuget.exe")
                            c.Arguments <- ("push "+ file+" -Source "+nugetServer))(TimeSpan.FromMinutes 5.0)
                       
                                                        
