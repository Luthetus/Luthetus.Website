using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.Wasm.FileSystem;

public class InMemoryEnvironmentProvider : IEnvironmentProvider
{
    public InMemoryEnvironmentProvider()
    {
        RootDirectoryAbsoluteFilePath = new AbsoluteFilePath(
            string.Empty,
            true,
            this);

        HomeDirectoryAbsoluteFilePath = new AbsoluteFilePath(
            "/Repos/",
            true,
            this);
    }

    public IAbsoluteFilePath RootDirectoryAbsoluteFilePath { get; }
    public IAbsoluteFilePath HomeDirectoryAbsoluteFilePath { get; }

    public char DirectorySeparatorChar => '/';
    public char AltDirectorySeparatorChar => '\\';

    public string GetRandomFileName()
    {
        return Guid.NewGuid().ToString();
    }

    public string JoinPaths(string pathOne, string pathTwo)
    {
        return string.Join(DirectorySeparatorChar, pathOne, pathTwo);
    }
}