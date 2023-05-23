using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

namespace Luthetus.Website.RazorLib.Repl.FileSystem;

public class ReplDirectoryHandler : IDirectoryHandler
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<ReplState> _replStateWrap;
    private readonly IDispatcher _dispatcher;

    public ReplDirectoryHandler(
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

    public Task CreateDirectoryAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        var replState = _replStateWrap.Value;

        var existingFile = replState.Files.FirstOrDefault(
            f => f.AbsoluteFilePathString == absoluteFilePathString);

        if (existingFile is not null)
            return Task.CompletedTask;

        _dispatcher.Dispatch(
            new ReplState.NextInstanceAction(
                inReplState =>
                {
                    var outDirectory = new ReplFile(
                        string.Empty,
                        absoluteFilePathString,
                        DateTime.UtcNow);

                    var outFiles = inReplState.Files.Add(
                        outDirectory);

                    return new ReplState(
                        inReplState.RootDirectory,
                        inReplState.DotNetSolution,
                        outFiles,
                        inReplState.ViewExplorerElementDimensions,
                        inReplState.TextEditorGroupElementDimensions);
                }));

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        string absoluteFilePathString,
        bool recursive,
        CancellationToken cancellationToken = default)
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
                    var outFiles = inReplState.Files.Remove(
                        existingFile);

                    return new ReplState(
                        inReplState.RootDirectory,
                        inReplState.DotNetSolution,
                        outFiles,
                        inReplState.ViewExplorerElementDimensions,
                        inReplState.TextEditorGroupElementDimensions);
                }));

        return Task.CompletedTask;
    }

    public Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string[]> GetDirectoriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        var replState = _replStateWrap.Value;

        var existingFile = replState.Files.FirstOrDefault(
            f => f.AbsoluteFilePathString == absoluteFilePathString);

        if (existingFile is null)
            return Task.FromResult(Array.Empty<string>());

        var childrenFromAllGenerations = replState.Files.Where(
            f => f.AbsoluteFilePathString.StartsWith(absoluteFilePathString) &&
                 f.AbsoluteFilePathString != absoluteFilePathString)
            .ToArray();

        var directChildren = childrenFromAllGenerations.Where(
            f =>
            {
                var withoutParentPrefix = new string(
                    f.AbsoluteFilePathString
                        .Skip(absoluteFilePathString.Length)
                        .ToArray());

                return withoutParentPrefix.EndsWith("/") &&
                       withoutParentPrefix.Count(x => x == '/') == 1;
            })
            .Select(f => f.AbsoluteFilePathString)
            .ToArray();

        return Task.FromResult(directChildren);
    }

    public Task<string[]> GetFilesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        var replState = _replStateWrap.Value;

        var existingFile = replState.Files.FirstOrDefault(
            f => f.AbsoluteFilePathString == absoluteFilePathString);

        if (existingFile is null)
            return Task.FromResult(Array.Empty<string>());

        var childrenFromAllGenerations = replState.Files.Where(
            f => f.AbsoluteFilePathString.StartsWith(absoluteFilePathString) &&
                 f.AbsoluteFilePathString != absoluteFilePathString);

        var directChildren = childrenFromAllGenerations.Where(
            f =>
            {
                var withoutParentPrefix = new string(
                    f.AbsoluteFilePathString
                        .Skip(absoluteFilePathString.Length)
                        .ToArray());

                return withoutParentPrefix.Count(x => x == '/') == 0;
            })
            .Select(f => f.AbsoluteFilePathString)
            .ToArray();

        return Task.FromResult(directChildren);
    }

    public async Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        var directories = await GetDirectoriesAsync(
            absoluteFilePathString,
            cancellationToken);

        var files = await GetFilesAsync(
            absoluteFilePathString,
            cancellationToken);

        return directories.Union(files);
    }
}