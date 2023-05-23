using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.TextEditor.RazorLib.Group;
using Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.Repl;

public partial class ReplTextEditorGroupDisplay : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public ReplState ReplState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public AppOptionsState AppOptionsState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public TextEditorGroupKey ReplTextEditorGroupKey { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
}