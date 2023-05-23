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
using Luthetus.Ide.ClassLib.DotNet;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Website.RazorLib.Repl.FolderExplorer;
using Luthetus.Ide.ClassLib.Menu;

namespace Luthetus.Website.RazorLib.Repl.SolutionExplorer;

public partial class ReplSolutionExplorerDisplay : ComponentBase, IDisposable
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
    [Inject]
    private ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;

    [CascadingParameter, EditorRequired]
    public ReplState ReplState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public AppOptionsState AppOptionsState { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public TextEditorGroupKey ReplTextEditorGroupKey { get; set; } = null!;
    [CascadingParameter(Name="ReplSolutionExplorerTreeViewStateKey"), EditorRequired]
    public TreeViewStateKey ReplSolutionExplorerTreeViewStateKey { get; set; } = null!;
    
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
        _treeViewKeyboardEventHandler = new ReplSolutionExplorerTreeViewKeyboardEventHandler(
            ReplTextEditorGroupKey,
            CommonMenuOptionsFactory,
            LuthetusIdeComponentRenderers,
            FileSystemProvider,
            Dispatcher,
            TreeViewService,
            TextEditorService,
            BackgroundTaskQueue);

        _treeViewMouseEventHandler = new ReplSolutionExplorerTreeViewMouseEventHandler(
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

    private async Task InitializeSolutionExplorerOnClickAsync()
    {
        await FileSystemProvider.File.WriteAllTextAsync(
            ReplState.INITAL_DOT_NET_SOLUTION_ABSOLUTE_FILE_PATH,
            ReplState.INITIAL_DOT_NET_SOLUTION_CONTENTS);

        await FileSystemProvider.File.WriteAllTextAsync(
            ReplState.INITAL_C_SHARP_PROJECT_ABSOLUTE_FILE_PATH,
            ReplState.INITAL_C_SHARP_PROJECT_CONTENTS);

        var dotNetSolutionAbsoluteFilePath = new AbsoluteFilePath(
            ReplState.INITAL_DOT_NET_SOLUTION_ABSOLUTE_FILE_PATH,
            false,
            EnvironmentProvider);

        var dotNetSolutionNamespacePath = new NamespacePath(
            string.Empty,
            dotNetSolutionAbsoluteFilePath);

        var dotNetSolution = DotNetSolutionParser.Parse(
            ReplState.INITIAL_DOT_NET_SOLUTION_CONTENTS,
            dotNetSolutionNamespacePath,
            EnvironmentProvider);

        Dispatcher.Dispatch(
            new ReplState.NextInstanceAction(inReplState =>
                new ReplState(
                    inReplState.RootDirectory,
                    dotNetSolution,
                    inReplState.Files,
                    inReplState.ViewExplorerElementDimensions,
                    inReplState.TextEditorGroupElementDimensions)));

        if (!TreeViewService.TryGetTreeViewState(
                ReplSolutionExplorerTreeViewStateKey,
                out _))
        {
            var rootTreeViewNode = new TreeViewSolution(
                dotNetSolution,
                LuthetusIdeComponentRenderers,
                FileSystemProvider,
                EnvironmentProvider,
                true,
                true);

            await rootTreeViewNode.LoadChildrenAsync();

            var treeViewState = new TreeViewState(
                ReplSolutionExplorerTreeViewStateKey,
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
                ReplSolutionExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        TreeViewService.TreeViewStateContainerWrap.StateChanged -= TreeViewStateContainerWrap_StateChanged;
    }
}