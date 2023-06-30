using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Website.RazorLib.Store.ReplCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.Repl.TextEditor;

public partial class ReplTextEditorGroupDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [CascadingParameter, EditorRequired]
    public ReplState ReplState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public AppOptionsState AppOptionsState { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
}