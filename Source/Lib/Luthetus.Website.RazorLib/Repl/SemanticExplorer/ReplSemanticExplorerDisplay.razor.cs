using Microsoft.AspNetCore.Components;
using Fluxor;
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
using Luthetus.Website.RazorLib.Store.ReplCase;
using Luthetus.Ide.ClassLib.Store.SemanticContextCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations.SemanticContext;

namespace Luthetus.Website.RazorLib.Repl.SemanticExplorer;

public partial class ReplSemanticExplorerDisplay : ComponentBase, IDisposable
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
    private IState<SemanticContextState> SemanticContextStateWrap { get; set; } = null!;

    [CascadingParameter, EditorRequired]
    public ReplState ReplState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public AppOptionsState AppOptionsState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public TextEditorGroupKey ReplTextEditorGroupKey { get; set; } = null!;
    [CascadingParameter(Name = "ReplSemanticExplorerTreeViewStateKey"), EditorRequired]
    public TreeViewStateKey ReplSemanticExplorerTreeViewStateKey { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;

    private static bool IsInitialized;

    private ITreeViewCommandParameter? _mostRecentTreeViewCommandParameter;

    private TreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
    private TreeViewMouseEventHandler _treeViewMouseEventHandler = null!;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsState.Options.IconSizeInPixels.GetValueOrDefault() *
        (2.0 / 3.0));

    protected override void OnInitialized()
    {
        _treeViewKeyboardEventHandler = new ReplSemanticExplorerTreeViewKeyboardEventHandler(
            ReplTextEditorGroupKey,
            Dispatcher,
            TreeViewService);

        _treeViewMouseEventHandler = new ReplSemanticExplorerTreeViewMouseEventHandler(
            ReplTextEditorGroupKey,
            Dispatcher,
            TreeViewService);

        TreeViewService.TreeViewStateContainerWrap.StateChanged += TreeViewStateContainerWrap_StateChanged;

        base.OnInitialized();
    }

    private async void TreeViewStateContainerWrap_StateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task InitializeSemanticExplorerOnClickAsync()
    {
        // Stop displaying button
        {
            IsInitialized = true;
            await InvokeAsync(StateHasChanged);
        }

        var semanticContextState = SemanticContextStateWrap.Value;

        if (!TreeViewService.TryGetTreeViewState(
                ReplSemanticExplorerTreeViewStateKey,
                out _))
        {
            var rootTreeViewNode = new TreeViewDotNetSolutionSemanticContext(
                (semanticContextState, semanticContextState.DotNetSolutionSemanticContext),
                LuthetusIdeComponentRenderers,
                FileSystemProvider,
                EnvironmentProvider,
                true,
                true);

            await rootTreeViewNode.LoadChildrenAsync();

            var treeViewState = new TreeViewState(
                ReplSemanticExplorerTreeViewStateKey,
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
                ReplSemanticExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        TreeViewService.TreeViewStateContainerWrap.StateChanged -= TreeViewStateContainerWrap_StateChanged;
    }
}