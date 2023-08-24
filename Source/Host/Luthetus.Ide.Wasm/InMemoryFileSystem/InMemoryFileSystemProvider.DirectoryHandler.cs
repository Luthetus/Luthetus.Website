using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using System.Collections.Immutable;
using System.Xml;

namespace Luthetus.Ide.Wasm.FileSystem;

public partial class InMemoryFileSystemProvider : IFileSystemProvider
{
    public class InMemoryDirectoryHandler : IDirectoryHandler
    {
        private readonly InMemoryFileSystemProvider _inMemoryFileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;
        private readonly IDispatcher _dispatcher;

        public InMemoryDirectoryHandler(
            InMemoryFileSystemProvider inMemoryFileSystemProvider,
            IEnvironmentProvider environmentProvider,
            IDispatcher dispatcher)
        {
            _inMemoryFileSystemProvider = inMemoryFileSystemProvider;
            _environmentProvider = environmentProvider;
            _dispatcher = dispatcher;
        }

        public Task<bool> ExistsAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_inMemoryFileSystemProvider._files.Any(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == absoluteFilePathString));
        }

        public async Task CreateDirectoryAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();

                await DoCreateDirectoryAsync(
                    absoluteFilePathString,
                    cancellationToken);
            }
            finally
            {
                _inMemoryFileSystemProvider._modificationSemaphore.Release();
            }
        }

        public async Task DeleteAsync(
            string absoluteFilePathString,
            bool recursive,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();

                await DoDeleteAsync(
                    absoluteFilePathString,
                    recursive,
                    cancellationToken);
            }
            finally
            {
                _inMemoryFileSystemProvider._modificationSemaphore.Release();
            }
        }

        public async Task CopyAsync(
            string sourceAbsoluteFilePathString,
            string destinationAbsoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();

                await DoCopyAsync(
                    sourceAbsoluteFilePathString,
                    destinationAbsoluteFilePathString,
                    cancellationToken);
            }
            finally
            {
                _inMemoryFileSystemProvider._modificationSemaphore.Release();
            }
        }
        
        public async Task MoveAsync(
            string sourceAbsoluteFilePathString,
            string destinationAbsoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();

                await DoMoveAsync(
                    sourceAbsoluteFilePathString,
                    destinationAbsoluteFilePathString,
                    cancellationToken);
            }
            finally
            {
                _inMemoryFileSystemProvider._modificationSemaphore.Release();
            }
        }

        public Task<string[]> GetDirectoriesAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            return DoGetDirectoriesAsync(
                absoluteFilePathString,
                cancellationToken);
        }

        public Task<string[]> GetFilesAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            return DoGetFilesAsync(
                absoluteFilePathString,
                cancellationToken);
        }

        public async Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();

                return await DoEnumerateFileSystemEntriesAsync(
                    absoluteFilePathString,
                    cancellationToken);
            }
            finally
            {
                _inMemoryFileSystemProvider._modificationSemaphore.Release();
            }
        }
        
        public Task<bool> DoExistsAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_inMemoryFileSystemProvider._files.Any(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == absoluteFilePathString));
        }

        public Task DoCreateDirectoryAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == absoluteFilePathString);

            if (existingFile is not null)
                return Task.CompletedTask;

            var absoluteFilePath = new AbsoluteFilePath(
                absoluteFilePathString,
                true,
                _environmentProvider);

            var outDirectory = new InMemoryFile(
                string.Empty,
                absoluteFilePath,
                DateTime.UtcNow);

            _inMemoryFileSystemProvider._files.Add(outDirectory);

            return Task.CompletedTask;
        }

        public async Task DoDeleteAsync(
            string absoluteFilePathString,
            bool recursive,
            CancellationToken cancellationToken = default)
        {
            if (absoluteFilePathString == _environmentProvider.RootDirectoryAbsoluteFilePath.GetAbsoluteFilePathString() ||
                absoluteFilePathString == _environmentProvider.HomeDirectoryAbsoluteFilePath.GetAbsoluteFilePathString())
            {
                return;
            }

            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == absoluteFilePathString);

            if (indexOfExistingFile == -1)
                return;

            var childFiles = _inMemoryFileSystemProvider._files.Where(imf =>
                imf.AbsoluteFilePath.GetAbsoluteFilePathString().StartsWith(absoluteFilePathString));

            foreach (var child in childFiles)
            {
                await _inMemoryFileSystemProvider.File.DeleteAsync(
                    child.AbsoluteFilePath.GetAbsoluteFilePathString(),
                    cancellationToken);
            }

            _inMemoryFileSystemProvider._files.RemoveAt(indexOfExistingFile);
        }

        public async Task DoCopyAsync(
            string sourceAbsoluteFilePathString,
            string destinationAbsoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == sourceAbsoluteFilePathString);

            if (indexOfExistingFile == -1)
                return;

            var childFiles = _inMemoryFileSystemProvider._files.Where(imf =>
                imf.AbsoluteFilePath.GetAbsoluteFilePathString().StartsWith(sourceAbsoluteFilePathString));

            var destinationAbsoluteFilePath = new AbsoluteFilePath(
                destinationAbsoluteFilePathString,
                true,
                _environmentProvider);

            var destinationFile = new InMemoryFile(
                string.Empty,
                destinationAbsoluteFilePath,
                DateTime.UtcNow);

            _inMemoryFileSystemProvider._files.Add(destinationFile);

            foreach (var child in childFiles)
            {
                var destinationChild = _environmentProvider.JoinPaths(
                    destinationAbsoluteFilePathString,
                    child.AbsoluteFilePath.FilenameWithExtension);

                await _inMemoryFileSystemProvider.File.CopyAsync(
                    child.AbsoluteFilePath.GetAbsoluteFilePathString(),
                    destinationChild,
                    cancellationToken);
            }
        }

        /// <summary>
        /// destinationAbsoluteFilePathString refers to the newly created dir not the parent dir which will contain it
        /// </summary>
        public async Task DoMoveAsync(
            string sourceAbsoluteFilePathString,
            string destinationAbsoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            if (sourceAbsoluteFilePathString == _environmentProvider.RootDirectoryAbsoluteFilePath.GetAbsoluteFilePathString() ||
                sourceAbsoluteFilePathString == _environmentProvider.HomeDirectoryAbsoluteFilePath.GetAbsoluteFilePathString())
            {
                return;
            }

            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == sourceAbsoluteFilePathString);

            if (indexOfExistingFile == -1)
                return;

            var childFiles = _inMemoryFileSystemProvider._files.Where(imf =>
                imf.AbsoluteFilePath.GetAbsoluteFilePathString().StartsWith(sourceAbsoluteFilePathString));

            var destinationAbsoluteFilePath = new AbsoluteFilePath(
                destinationAbsoluteFilePathString,
                true,
                _environmentProvider);

            var destinationFile = new InMemoryFile(
                string.Empty,
                destinationAbsoluteFilePath,
                DateTime.UtcNow);

            _inMemoryFileSystemProvider._files.Add(destinationFile);

            foreach (var child in childFiles)
            {
                var destinationChild = _environmentProvider.JoinPaths(
                    destinationAbsoluteFilePathString,
                    child.AbsoluteFilePath.FilenameWithExtension);

                await _inMemoryFileSystemProvider.File.MoveAsync(
                    child.AbsoluteFilePath.GetAbsoluteFilePathString(),
                    destinationChild,
                    cancellationToken);
            }

            _inMemoryFileSystemProvider._files.RemoveAt(indexOfExistingFile);
        }

        public Task<string[]> DoGetDirectoriesAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == absoluteFilePathString);

            if (existingFile is null)
                return Task.FromResult(Array.Empty<string>());

            var childrenFromAllGenerations = _inMemoryFileSystemProvider._files.Where(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString().StartsWith(absoluteFilePathString) &&
                     f.AbsoluteFilePath.GetAbsoluteFilePathString() != absoluteFilePathString)
                .ToArray();

            var directChildren = childrenFromAllGenerations.Where(
                f =>
                {
                    var withoutParentPrefix = new string(
                        f.AbsoluteFilePath.GetAbsoluteFilePathString()
                            .Skip(absoluteFilePathString.Length)
                            .ToArray());

                    return withoutParentPrefix.EndsWith("/") &&
                           withoutParentPrefix.Count(x => x == '/') == 1;
                })
                .Select(f => f.AbsoluteFilePath.GetAbsoluteFilePathString())
                .ToArray();

            return Task.FromResult(directChildren);
        }

        public Task<string[]> DoGetFilesAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == absoluteFilePathString);

            if (existingFile is null)
                return Task.FromResult(Array.Empty<string>());

            var childrenFromAllGenerations = _inMemoryFileSystemProvider._files.Where(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString().StartsWith(absoluteFilePathString) &&
                     f.AbsoluteFilePath.GetAbsoluteFilePathString() != absoluteFilePathString);

            var directChildren = childrenFromAllGenerations.Where(
                f =>
                {
                    var withoutParentPrefix = new string(
                        f.AbsoluteFilePath.GetAbsoluteFilePathString()
                            .Skip(absoluteFilePathString.Length)
                            .ToArray());

                    return withoutParentPrefix.Count(x => x == '/') == 0;
                })
                .Select(f => f.AbsoluteFilePath.GetAbsoluteFilePathString())
                .ToArray();

            return Task.FromResult(directChildren);
        }

        public async Task<IEnumerable<string>> DoEnumerateFileSystemEntriesAsync(
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
}