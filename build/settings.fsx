#r "tools/FAKE/tools/FakeLib.dll"
open Fake

let projName="SqlFu"
let projDir= "..\src" @@ projName
let testDir="..\src" @@ "Tests"

let sqlTestDir= testDir + ".SqlServer"

let localNugetRepo="E:/Libs/nuget"
let nugetExeDir="tools"



