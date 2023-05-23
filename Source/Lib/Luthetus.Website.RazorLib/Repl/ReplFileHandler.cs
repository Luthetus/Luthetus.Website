using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

namespace Luthetus.Website.RazorLib.Repl;

public class ReplFileHandler : IFileHandler
{
    private readonly IState<ReplState> _replStateWrap;

    public ReplFileHandler(IState<ReplState> replStateWrap)
    {
        _replStateWrap = replStateWrap;
    }

    public Task<bool> ExistsAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task CopyAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<DateTime> GetLastWriteTimeAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<string> ReadAllTextAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task WriteAllTextAsync(
        string absoluteFilePathString,
        string contents,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}