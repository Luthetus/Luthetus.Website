using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

[FeatureState]
public partial class ReplState
{
    private ReplState()
    {
    }
    
    public ReplState(IAbsoluteFilePath rootDirectory)
    {
        RootDirectory = rootDirectory;
    }

    public IAbsoluteFilePath? RootDirectory { get; }
}
