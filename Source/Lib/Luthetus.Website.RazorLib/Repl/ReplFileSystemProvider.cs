using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

namespace Luthetus.Website.RazorLib.Repl;

public class ReplFileSystemProvider : IFileSystemProvider
{
    private readonly IState<ReplState> _replStateWrap;

    public ReplFileSystemProvider(IState<ReplState> replStateWrap)
    {
        _replStateWrap = replStateWrap;

        File = new ReplFileHandler(_replStateWrap);
        Directory = new ReplDirectoryHandler(_replStateWrap);
    }

    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
}