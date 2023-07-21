using Luthetus.Website.RazorLib.Store.ReplCase;

namespace Luthetus.Website.RazorLib.Repl.FileSystem;

public class ReplFileHandler : IFileHandler
{
    private readonly ReplFileSystemProvider _replFileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<ReplState> _replStateWrap;
    private readonly IDispatcher _dispatcher;

    public ReplFileHandler(
        ReplFileSystemProvider replFileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IState<ReplState> replStateWrap,
        IDispatcher dispatcher)
    {
        _replFileSystemProvider = replFileSystemProvider;
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

        // Ensure Parent Directories Exist
        {
            var parentDirectories = absoluteFilePathString
                .Split("/")
                // The root directory splits into string.Empty
                .Skip(1)
                // Skip the file being written to itself
                .SkipLast(1)
                .ToArray();

            var directoryPathBuilder = new StringBuilder("/");

            for (int i = 0; i < parentDirectories.Length; i++)
            {
                directoryPathBuilder.Append(parentDirectories[i]);
                directoryPathBuilder.Append("/");

                _replFileSystemProvider.Directory.CreateDirectoryAsync(
                    directoryPathBuilder.ToString());
            }
        }

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