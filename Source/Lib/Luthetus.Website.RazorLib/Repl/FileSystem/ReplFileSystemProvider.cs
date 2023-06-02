using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Store.ReplCase;

namespace Luthetus.Website.RazorLib.Repl.FileSystem;

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
            this,
            _environmentProvider,
            _replStateWrap,
            _dispatcher);

        Directory = new ReplDirectoryHandler(
            this,
            _environmentProvider,
            _replStateWrap,
            _dispatcher);
    }

    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
}