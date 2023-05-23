using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;
using System.Collections.Immutable;

namespace Luthetus.Website.RazorLib.Repl.FileSystem;

public class ReplFileHandler : IFileHandler
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<ReplState> _replStateWrap;
    private readonly IDispatcher _dispatcher;

    public ReplFileHandler(
        IEnvironmentProvider environmentProvider,
        IState<ReplState> replStateWrap,
        IDispatcher dispatcher)
    {
        _environmentProvider = environmentProvider;
        _replStateWrap = replStateWrap;
        _dispatcher = dispatcher;
    }

    public Task<bool> ExistsAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        var replState = _replStateWrap.Value;

        return Task.FromResult(replState.Files.Any(
            f => f.AbsoluteFilePathString == absoluteFilePathString));
    }

    public Task DeleteAsync(string absoluteFilePathString, CancellationToken cancellationToken = default)
    {
        var replState = _replStateWrap.Value;

        var existingFile = replState.Files.FirstOrDefault(
            f => f.AbsoluteFilePathString == absoluteFilePathString);

        if (existingFile is null)
            return Task.CompletedTask;

        _dispatcher.Dispatch(
            new ReplState.NextInstanceAction(
                inReplState =>
                {
                    var outFiles = inReplState.Files.Remove(existingFile);

                    return new ReplState(
                        inReplState.RootDirectory,
                        inReplState.DotNetSolution,
                        outFiles,
                        inReplState.ViewExplorerElementDimensions,
                        inReplState.TextEditorGroupElementDimensions);
                }));

        return Task.CompletedTask;
    }

    public async Task CopyAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(CopyAsync));

        throw new NotImplementedException();
    }

    public async Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(MoveAsync));

        throw new NotImplementedException();
    }

    public Task<DateTime> GetLastWriteTimeAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        var replState = _replStateWrap.Value;

        var existingFile = replState.Files.FirstOrDefault(
            f => f.AbsoluteFilePathString == absoluteFilePathString);

        if (existingFile is null)
            return Task.FromResult(default(DateTime));

        return Task.FromResult(existingFile.LastModifiedDateTime);
    }

    public Task<string> ReadAllTextAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        var replState = _replStateWrap.Value;

        var existingFile = replState.Files.FirstOrDefault(
            f => f.AbsoluteFilePathString == absoluteFilePathString);

        if (existingFile is null)
            return Task.FromResult(string.Empty);

        return Task.FromResult(existingFile.Data);
    }

    public Task WriteAllTextAsync(
        string absoluteFilePathString,
        string contents,
        CancellationToken cancellationToken = default)
    {
        var replState = _replStateWrap.Value;

        var existingFile = replState.Files.FirstOrDefault(
            f => f.AbsoluteFilePathString == absoluteFilePathString);

        _dispatcher.Dispatch(
            new ReplState.NextInstanceAction(
                inReplState =>
                {
                    var outFile = new ReplFile(
                        contents,
                        absoluteFilePathString,
                        DateTime.UtcNow);

                    ImmutableList<ReplFile> nextFiles;

                    if (existingFile is null)
                    {
                        nextFiles = inReplState.Files.Add(
                            outFile);
                    }
                    else
                    {
                        nextFiles = inReplState.Files.Replace(
                            existingFile,
                            outFile);
                    }

                    return new ReplState(
                        inReplState.RootDirectory,
                        inReplState.DotNetSolution,
                        nextFiles,
                        inReplState.ViewExplorerElementDimensions,
                        inReplState.TextEditorGroupElementDimensions);
                }));

        return Task.CompletedTask;
    }
}