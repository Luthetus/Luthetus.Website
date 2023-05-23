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
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Common.RazorLib.BackgroundTaskCase;

namespace Luthetus.Website.RazorLib.Repl;

public partial class ReplFolderExplorerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; set; } = null!;
    [Inject]
    private IBackgroundTaskQueue BackgroundTaskQueue { get; set; } = null!;

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

    private TreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
    private TreeViewMouseEventHandler _treeViewMouseEventHandler = null!;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsState.Options.IconSizeInPixels.GetValueOrDefault() *
        (2.0 / 3.0));

    protected override void OnInitialized()
    {
        _treeViewKeyboardEventHandler = new ReplTreeViewKeyboardEventHandler(
            ReplTextEditorGroupKey,
            LuthetusIdeComponentRenderers,
            FileSystemProvider,
            Dispatcher,
            TreeViewService,
            TextEditorService,
            BackgroundTaskQueue);

        _treeViewMouseEventHandler = new ReplTreeViewMouseEventHandler(
            ReplTextEditorGroupKey,
            Dispatcher,
            TextEditorService,
            LuthetusIdeComponentRenderers,
            FileSystemProvider,
            TreeViewService,
            BackgroundTaskQueue);

        TreeViewService.TreeViewStateContainerWrap.StateChanged += TreeViewStateContainerWrap_StateChanged;

        base.OnInitialized();
    }

    private async void TreeViewStateContainerWrap_StateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private void InitializeRootDirectoryOnClick()
    {
        Dispatcher.Dispatch(
            new ReplState.NextInstanceAction(inReplState =>
                new ReplState(
                    EnvironmentProvider.RootDirectoryAbsoluteFilePath,
                    inReplState.Files,
                    inReplState.FolderExplorerElementDimensions,
                    inReplState.TextEditorGroupElementDimensions)));

        if (!TreeViewService.TryGetTreeViewState(
                ReplTreeViewStateKey,
                out _))
        {
            var rootTreeViewNode = new TreeViewAbsoluteFilePath(
                EnvironmentProvider.RootDirectoryAbsoluteFilePath,
                LuthetusIdeComponentRenderers,
                FileSystemProvider,
                EnvironmentProvider,
                true,
                true);

            var treeViewState = new TreeViewState(
                ReplTreeViewStateKey,
                rootTreeViewNode,
                rootTreeViewNode,
                ImmutableList<TreeViewNoType>.Empty);

            TreeViewService.RegisterTreeViewState(treeViewState);
        }
    }

    private async Task OnTreeViewContextMenuFunc(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewCommandParameter = treeViewCommandParameter;

        Dispatcher.Dispatch(
            new DropdownsState.AddActiveAction(
                ReplContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        TreeViewService.TreeViewStateContainerWrap.StateChanged -= TreeViewStateContainerWrap_StateChanged;
    }
}