using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.Wasm.FileSystem;

public class InMemoryFile
{
    public InMemoryFile(
        string data,
        IAbsoluteFilePath absoluteFilePath,
        DateTime lastModifiedDateTime)
    {
        Data = data;
        AbsoluteFilePath = absoluteFilePath;
        LastModifiedDateTime = lastModifiedDateTime;
    }

    public string Data { get; }
    public IAbsoluteFilePath AbsoluteFilePath { get; }
    public DateTime LastModifiedDateTime { get; }
}