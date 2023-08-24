using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using System.Collections.Immutable;
using System.Text;
using System.Xml;

namespace Luthetus.Ide.Wasm.FileSystem;

public partial class InMemoryFileSystemProvider : IFileSystemProvider
{
    public class InMemoryFileHandler : IFileHandler
    {
        private readonly InMemoryFileSystemProvider _inMemoryFileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;
        private readonly IDispatcher _dispatcher;

        public InMemoryFileHandler(
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
            return DoExistsAsync(absoluteFilePathString, cancellationToken);
        }

        public async Task DeleteAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();
                
                await DoDeleteAsync(
                    absoluteFilePathString,
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

        public async Task<DateTime> GetLastWriteTimeAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            return await DoGetLastWriteTimeAsync(
                absoluteFilePathString,
                cancellationToken);
        }

        public async Task<string> ReadAllTextAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            return await DoReadAllTextAsync(
                absoluteFilePathString,
                cancellationToken);
        }

        public async Task WriteAllTextAsync(
            string absoluteFilePathString,
            string contents,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();
                
                await DoWriteAllTextAsync(
                    absoluteFilePathString,
                    contents,
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
                imf => imf.AbsoluteFilePath.GetAbsoluteFilePathString() == absoluteFilePathString));
        }

        public Task DoDeleteAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == absoluteFilePathString);

            if (indexOfExistingFile == -1)
                return Task.CompletedTask;

            _inMemoryFileSystemProvider._files.RemoveAt(indexOfExistingFile);

            return Task.CompletedTask;
        }

        public async Task DoCopyAsync(
            string sourceAbsoluteFilePathString,
            string destinationAbsoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            // Source
            {
                var indexOfSource = _inMemoryFileSystemProvider._files.FindIndex(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == sourceAbsoluteFilePathString);

                if (indexOfSource == -1)
                    throw new ApplicationException($"Source file: {sourceAbsoluteFilePathString} was not found.");
            }

            // Destination
            { 
                var indexOfDestination = _inMemoryFileSystemProvider._files.FindIndex(
                    f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == destinationAbsoluteFilePathString);

                if (indexOfDestination != -1)
                    throw new ApplicationException($"A file already exists with the path: {sourceAbsoluteFilePathString}.");
            }

            var contents = await DoReadAllTextAsync(
                sourceAbsoluteFilePathString,
                cancellationToken);

            await DoWriteAllTextAsync(
                destinationAbsoluteFilePathString,
                contents,
                cancellationToken);
        }

        public async Task DoMoveAsync(
            string sourceAbsoluteFilePathString,
            string destinationAbsoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            await DoCopyAsync(
                sourceAbsoluteFilePathString,
                destinationAbsoluteFilePathString,
                cancellationToken);

            await DoDeleteAsync(
                sourceAbsoluteFilePathString,
                cancellationToken);
        }

        public Task<DateTime> DoGetLastWriteTimeAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == absoluteFilePathString);

            if (existingFile is null)
                return Task.FromResult(default(DateTime));

            return Task.FromResult(existingFile.LastModifiedDateTime);
        }

        public Task<string> DoReadAllTextAsync(
            string absoluteFilePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == absoluteFilePathString);

            if (existingFile is null)
                return Task.FromResult(string.Empty);

            return Task.FromResult(existingFile.Data);
        }

        public Task DoWriteAllTextAsync(
            string absoluteFilePathString,
            string contents,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(
                f => f.AbsoluteFilePath.GetAbsoluteFilePathString() == absoluteFilePathString);

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

                    _inMemoryFileSystemProvider.Directory.CreateDirectoryAsync(
                        directoryPathBuilder.ToString());
                }
            }

            var absoluteFilePath = new AbsoluteFilePath(
                absoluteFilePathString,
                false,
                _environmentProvider);

            var outFile = new InMemoryFile(
                contents,
                absoluteFilePath,
                DateTime.UtcNow);

            _inMemoryFileSystemProvider._files.Add(outFile);

            return Task.CompletedTask;
        }
    }
}