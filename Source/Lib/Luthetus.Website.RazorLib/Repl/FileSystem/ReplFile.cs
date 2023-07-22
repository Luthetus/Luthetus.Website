namespace Luthetus.Website.RazorLib.Repl.FileSystem;

public class ReplFile
{
    public ReplFile(
        string data,
        string absoluteFilePathString,
        DateTime lastModifiedDateTime)
    {
        Data = data;
        AbsoluteFilePathString = absoluteFilePathString;
        LastModifiedDateTime = lastModifiedDateTime;
    }

    public string Data { get; }
    public string AbsoluteFilePathString { get; }
    public DateTime LastModifiedDateTime { get; }
}