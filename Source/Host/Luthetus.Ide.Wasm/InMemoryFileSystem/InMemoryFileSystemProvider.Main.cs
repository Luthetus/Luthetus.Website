using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using System.Collections.Immutable;

namespace Luthetus.Ide.Wasm.FileSystem;

public partial class InMemoryFileSystemProvider : IFileSystemProvider
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDispatcher _dispatcher;
    private readonly List<InMemoryFile> _files = new();
    private readonly SemaphoreSlim _modificationSemaphore = new(1, 1);

    public InMemoryFileSystemProvider(
        IEnvironmentProvider environmentProvider,
        IDispatcher dispatcher)
    {
        _environmentProvider = environmentProvider;
        _dispatcher = dispatcher;

        File = new InMemoryFileHandler(
            this,
            _environmentProvider,
            _dispatcher);

        Directory = new InMemoryDirectoryHandler(
            this,
            _environmentProvider,
            _dispatcher);

        Directory
            .CreateDirectoryAsync(_environmentProvider.RootDirectoryAbsoluteFilePath.GetAbsoluteFilePathString())
            .Wait();

        Directory
            .CreateDirectoryAsync(_environmentProvider.HomeDirectoryAbsoluteFilePath.GetAbsoluteFilePathString())
            .Wait();
    }

    public ImmutableArray<InMemoryFile> Files => _files.ToImmutableArray();

    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
}