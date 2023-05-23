using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

namespace Luthetus.Website.RazorLib.Repl;

public class ReplFileSystemProvider : IFileSystemProvider
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<ReplState> _replStateWrap;
    private readonly IDispatcher _dispatcher;

    public ReplFileSystemProvider(
        IEnvironmentProvider environmentProvider,
        IState<ReplState> replStateWrap,
        IDispatcher dispatcher)
    {
        _environmentProvider = environmentProvider;
        _replStateWrap = replStateWrap;
        _dispatcher = dispatcher;

        File = new ReplFileHandler(
            _environmentProvider,
            _replStateWrap,
            _dispatcher);

        Directory = new ReplDirectoryHandler(
            _environmentProvider,
            _replStateWrap,
            _dispatcher);
    }

    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
}