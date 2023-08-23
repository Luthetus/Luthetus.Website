using Luthetus.Website.RazorLib.Store.ReplCase;
using Luthetus.Website.RazorLib.Facts;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.Store.DropdownCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.ComponentRenderers;

namespace Luthetus.Website.RazorLib.Repl.FolderExplorer;

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
    private ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;

    [CascadingParameter, EditorRequired]
    public ReplState ReplState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public AppOptionsState AppOptionsState { get; set; } = null!;

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
        _treeViewKeyboardEventHandler = new ReplFolderExplorerTreeViewKeyboardEventHandler(
            Dispatcher,
            TreeViewService);

        _treeViewMouseEventHandler = new ReplFolderExplorerTreeViewMouseEventHandler(
            Dispatcher,
            TreeViewService);

        TreeViewService.TreeViewStateContainerWrap.StateChanged += TreeViewStateContainerWrap_StateChanged;

        base.OnInitialized();
    }

    private async void TreeViewStateContainerWrap_StateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task InitializeFolderExplorerOnClickAsync()
    {
        Dispatcher.Dispatch(
            new ReplState.NextInstanceAction(inReplState =>
                new ReplState(
                    EnvironmentProvider.RootDirectoryAbsoluteFilePath,
                    inReplState.DotNetSolution,
                    inReplState.Files,
                    inReplState.ViewExplorerElementDimensions,
                    inReplState.TextEditorGroupElementDimensions)));

        if (!TreeViewService.TryGetTreeViewState(
                ReplFacts.TreeViewStateKeys.FolderExplorer,
                out _))
        {
            var rootTreeViewNode = new TreeViewAbsoluteFilePath(
                EnvironmentProvider.RootDirectoryAbsoluteFilePath,
                LuthetusIdeComponentRenderers,
                LuthetusCommonComponentRenderers,
                FileSystemProvider,
                EnvironmentProvider,
                true,
                true);

            await rootTreeViewNode.LoadChildrenAsync();

            var treeViewState = new TreeViewState(
                ReplFacts.TreeViewStateKeys.FolderExplorer,
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
                ReplFolderExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        TreeViewService.TreeViewStateContainerWrap.StateChanged -= TreeViewStateContainerWrap_StateChanged;
    }
}