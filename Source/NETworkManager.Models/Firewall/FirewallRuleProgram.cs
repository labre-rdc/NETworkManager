using System.IO;

namespace NETworkManager.Models.Firewall;

/// <summary>
/// Represents a program associated with a firewall rule and stores information about its
/// executable file and directory.
/// </summary>
/// <remarks>
/// This class encapsulates details of an executable file used in defining firewall rules.
/// It validates the existence of the specified executable file and stores its path and directory.
/// </remarks>
public class FirewallRuleProgram
{
    #region Variables

    /// <summary>
    /// Gets or sets the file system path.
    /// </summary>
    /// <remarks>
    /// The value of this property represents a file or directory path
    /// in a string format. It can be an absolute or relative path,
    /// depending on the application context.
    /// Ensure that the path is correctly formatted and accessible
    /// to avoid runtime errors.
    /// </remarks>
    public DirectoryInfo Path { private set; get; }

    /// <summary>
    /// Represents the file information for the executable associated with the firewall rule program.
    /// </summary>
    /// <remarks>
    /// This property provides access to the executable file for which the firewall rule is being applied.
    /// It ensures that the file exists when the object is created and is part of the validation process.
    /// </remarks>
    public FileInfo Executable { private set; get; }
    
    #endregion
    
    #region Constructor

    /// Represents a program associated with a firewall rule, including its executable file and directory path.
    /// Used for defining and validating the program tied to specific firewall rules.
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