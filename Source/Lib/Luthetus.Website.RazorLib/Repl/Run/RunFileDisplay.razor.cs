namespace Luthetus.Website.RazorLib.Repl.Run;

public partial class RunFileDisplay : IRunFileDisplayRenderer
{
    [Parameter, EditorRequired]
    public string SourceText { get; set; } = null!;
}