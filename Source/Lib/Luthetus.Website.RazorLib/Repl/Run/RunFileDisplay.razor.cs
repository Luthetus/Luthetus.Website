using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.Repl.Run;

public partial class RunFileDisplay : IRunFileDisplayRenderer
{
    [Parameter, EditorRequired]
    public string SourceText { get; set; } = null!;
}