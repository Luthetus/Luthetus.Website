using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Website.RazorLib.Repl.FileSystem;

public class ReplEnvironmentProvider : IEnvironmentProvider
{
    public ReplEnvironmentProvider()
    {
    }

    public IAbsoluteFilePath RootDirectoryAbsoluteFilePath
    {
        get
        {
            return new AbsoluteFilePath(
                string.Empty,
                true,
                this);
        }
    }

    public IAbsoluteFilePath HomeDirectoryAbsoluteFilePath
    {
        get
        {
            return RootDirectoryAbsoluteFilePath;
        }
    }

    public char DirectorySeparatorChar => '/';
    public char AltDirectorySeparatorChar => '\\';

    public string GetRandomFileName()
    {
        return Guid.NewGuid().ToString();
    }
}