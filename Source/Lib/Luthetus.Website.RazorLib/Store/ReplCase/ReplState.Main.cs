using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Repl;
using System.Collections.Immutable;

namespace Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

[FeatureState]
public partial class ReplState
{
    private ReplState()
    {
        Files = ImmutableList<ReplFile>.Empty;
    }

    public ReplState(
        IAbsoluteFilePath rootDirectory,
        ImmutableList<ReplFile> files)
    {
        RootDirectory = rootDirectory;
        Files = files;
    }

    public IAbsoluteFilePath? RootDirectory { get; }
    public ImmutableList<ReplFile> Files { get; }
}
