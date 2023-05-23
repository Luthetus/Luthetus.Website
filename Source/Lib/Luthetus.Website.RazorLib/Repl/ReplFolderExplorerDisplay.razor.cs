using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.TextEditor.RazorLib.Group;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Website.RazorLib.Repl;

public partial class ReplFolderExplorerDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [CascadingParameter, EditorRequired]
    public ReplState ReplState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public TextEditorGroupKey ReplTextEditorGroupKey { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;

    private void InitializeRootDirectoryOnClick()
    {
        Dispatcher.Dispatch(
            new ReplState.NextInstanceAction(inReplState =>
                new ReplState(EnvironmentProvider.RootDirectoryAbsoluteFilePath)));
    }
}