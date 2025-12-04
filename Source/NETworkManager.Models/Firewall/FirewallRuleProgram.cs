using System.IO;

namespace NETworkManager.Models.Firewall;

public class FirewallRuleProgram
{
    #region Variables
    
    public DirectoryInfo Path { private set; get; }
    
    public FileInfo Executable { private set; get; }
    
    #endregion
    
    #region Constructor

    public FirewallRuleProgram(string pathToExe)
    {
        var exe = new FileInfo(pathToExe);
        if (!exe.Exists)
            throw new FileNotFoundException(("The executable you entered was not found."));
        Executable = exe;
        if (exe.DirectoryName is not null)
            Path = new DirectoryInfo(exe.DirectoryName);
    }
    
    #endregion
}