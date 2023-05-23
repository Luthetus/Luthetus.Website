using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.TextEditor.RazorLib.Group;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.Store.DropdownCase;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView;

namespace Luthetus.Website.RazorLib.Repl;

public partial class ReplFolderExplorerDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

    [CascadingParameter, EditorRequired]
    public ReplState ReplState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public AppOptionsState AppOptionsState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public TextEditorGroupKey ReplTextEditorGroupKey { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public TreeViewStateKey ReplTreeViewStateKey { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;

    private ITreeViewCommandParameter? _mostRecentTreeViewCommandParameter;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsState.Options.IconSizeInPixels.GetValueOrDefault() *
        (2.0 / 3.0));

    private void InitializeRootDirectoryOnClick()
    {
        Dispatcher.Dispatch(
            new ReplState.NextInstanceAction(inReplState =>
                new ReplState(EnvironmentProvider.RootDirectoryAbsoluteFilePath)));
    }

    private async Task OnTreeViewContextMenuFunc(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewCommandParameter = treeViewCommandParameter;

        Dispatcher.Dispatch(
            new DropdownsState.AddActiveAction(
                ReplContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }
}