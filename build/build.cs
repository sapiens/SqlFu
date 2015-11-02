
using System;
using System.IO;
using MakeSharp;
using MakeSharp.MsBuild;


//class h
//{
//    void b()
//    {
        Project.StaticName = "Sqlfu";
        Solution.FileName = @"..\src\SqlFu.sln";  
            
		Project.Current.AssemblyExtension = "dll";


//    }
//}



public class clean
 {
     public void Run()
     {

	  BuildScript.TempDirectory.CleanupDir();
      
      Solution.Instance.FilePath.MsBuildClean();
    }

   
}

public class UpdateVersion
{

    
    public ITaskContext Context { get; set; }
    public void Run() { 
    var ver = Context.InitData.Get<string>("v");
		if (ver == null)
        {
			//bump=minor|patch
			var bump=Context.InitData.Get<string>("bump");			
			ver=GetVersion(bump);
			if (bump==null) return;
        }
        var info = Project.Current.GetAssemblyInfo();
       
       
        info.Info.Version = info.Info.FileVersion = ver;
        info.Save();
      
        ("Version updated to "+ver).ToConsole();
        Context.Data["version"] = ver;
    }

    string GetVersion(string bump=null)
    {
        var info=Project.Current.GetAssemblyInfo();
		if (bump=="minor") info.Info.BumpMinorVersion();
		if (bump=="patch") info.Info.BumpPatchVersion();
		Context.Data["version"] = info.Info.Version;
        ("Using version "+info.Info.Version).ToConsole();
		return info.Info.Version;
    }
}

[Default]
[Depends("clean","UpdateVersion")]
public class build
{
    public ITaskContext Context { get; set; }

    public void Run()
    {
        Solution.Instance.FilePath.MsBuildRelease();
    }
}


[Depends("build")]
public class pack
{
    public ITaskContext Context { get; set; }


    public void Run()
    {
        "template.nuspec".CreateNuget(s =>
        {
            s.Metadata.Version = Context.Data["version"].ToString();
			if (Context.InitData.HasArgument("pre"))
            {
                s.Metadata.Version +="-"+Context.InitData.Get<string>("pre")?? "pre";
				if (Context.InitData.HasArgument("bld"))
				{
					s.Metadata.Version +="-"+DateTime.Now.ToString("yyyyMMddHHmm");
				}
            }
			
			var depVers=new Dictionary<string,string>();
			
			//depVers["SqlFu"]="3.0.0-alpha-build20150919";
			
            foreach (var dep in Project.Current.DepsList)
            {
							
			   var ver = Project.Current.ReleasePathForAssembly(dep+".dll").GetAssemblyVersion();
                s.AddDependency(dep, depVers.GetValueOrDefault(dep,ver.ToString(3)));
            }
                      
        }, p =>
        {
            p.OutputDir = BuildScript.TempDirName;
            p.BasePath = Project.Current.Directory;
          
            p.BuildSymbols = true;
            p.Publish = Context.InitData.HasArgument("push");

        });
        Context.Data["pkg"] = Path.Combine(BuildScript.TempDirName,
            Project.Current.Name + "." + Context.Data["version"] + ".nupkg");
    }

}

[Depends("pack")]
public class local
{

    public ITaskContext Context { get; set; }
    public void Run()
    {
        string[] args=new string[3];
        args[0]=Path.Combine(BuildScript.TempDirName, "*.nupkg");
        args[1] = @"e:\Libs\nuget";
        args[2]="/Y";
        "xcopy".Exec(args);

    }
}








