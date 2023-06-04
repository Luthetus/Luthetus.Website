using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Website.RazorLib.Store.ReplCase.Facts;
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
using Luthetus.Ide.ClassLib.DotNet;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Website.RazorLib.Store.ReplCase;
using Luthetus.Ide.ClassLib.Store.SemanticContextCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Implementations;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Keys;

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
    private ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; set; } = null!;

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
            Dispatcher,
            TreeViewService);

        _treeViewMouseEventHandler = new ReplSolutionExplorerTreeViewMouseEventHandler(
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

    private async Task InitializeSolutionExplorerOnClickAsync()
    {
        // WriteAllTextAsync
        {
            // AppCss
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.APP_CSS_ABSOLUTE_FILE_PATH,
                ReplStateFacts.APP_CSS_CONTENTS);

            // AppRazor
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.APP_RAZOR_FILE_ABSOLUTE_FILE_PATH,
                ReplStateFacts.APP_RAZOR_FILE_CONTENTS);

            // TODO: (2023-06-03) Decide whether to delete CounterTest considering I'm adding "PersonDisplay" markup and codebehind
            //
            //// CounterTest
            //await FileSystemProvider.File.WriteAllTextAsync(
            //    ReplStateFacts.COUNTER_TEST_ABSOLUTE_FILE_PATH,
            //    ReplStateFacts.COUNTER_TEST_CONTENTS);

            // Csproj
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.C_SHARP_PROJECT_ABSOLUTE_FILE_PATH,
                ReplStateFacts.C_SHARP_PROJECT_CONTENTS);

            // Imports
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.IMPORTS_RAZOR_FILE_ABSOLUTE_FILE_PATH,
                ReplStateFacts.IMPORTS_RAZOR_FILE_CONTENTS);

            // IndexHtml
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.INDEX_HTML_FILE_ABSOLUTE_FILE_PATH,
                ReplStateFacts.INDEX_HTML_FILE_CONTENTS);

            // IndexRazor
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.INDEX_RAZOR_FILE_ABSOLUTE_FILE_PATH,
                ReplStateFacts.INDEX_RAZOR_FILE_CONTENTS);
            
            // IPersonModel
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.IPERSON_MODEL_ABSOLUTE_FILE_PATH,
                ReplStateFacts.IPERSON_MODEL_CONTENTS);

            // MainLayout
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.MAIN_LAYOUT_RAZOR_FILE_ABSOLUTE_FILE_PATH,
                ReplStateFacts.MAIN_LAYOUT_RAZOR_FILE_CONTENTS);

            // PersonModel
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.PERSON_DISPLAY_MARKUP_ABSOLUTE_FILE_PATH,
                ReplStateFacts.PERSON_DISPLAY_MARKUP_CONTENTS);

            // PersonModel
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.PERSON_DISPLAY_CODEBEHIND_ABSOLUTE_FILE_PATH,
                ReplStateFacts.PERSON_DISPLAY_CODEBEHIND_CONTENTS);

            // PersonModel
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.PERSON_MODEL_ABSOLUTE_FILE_PATH,
                ReplStateFacts.PERSON_MODEL_CONTENTS);

            // Program
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.PROGRAM_CS_FILE_ABSOLUTE_FILE_PATH,
                ReplStateFacts.PROGRAM_CS_FILE_CONTENTS);

            // Sln
            await FileSystemProvider.File.WriteAllTextAsync(
                ReplStateFacts.SLN_ABSOLUTE_FILE_PATH,
                ReplStateFacts.SLN_CONTENTS);
        }

        var dotNetSolutionAbsoluteFilePath = new AbsoluteFilePath(
            ReplStateFacts.SLN_ABSOLUTE_FILE_PATH,
            false,
            EnvironmentProvider);

        var dotNetSolutionNamespacePath = new NamespacePath(
            string.Empty,
            dotNetSolutionAbsoluteFilePath);

        var dotNetSolution = DotNetSolutionParser.Parse(
            ReplStateFacts.SLN_CONTENTS,
            dotNetSolutionNamespacePath,
            EnvironmentProvider);

        var dotNetSolutionSemanticContext = new DotNetSolutionSemanticContext(
            DotNetSolutionKey.NewSolutionKey(),
            dotNetSolution);

        Dispatcher.Dispatch(
            new SemanticContextState.SetDotNetSolutionSemanticContextAction(
                dotNetSolutionSemanticContext));

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