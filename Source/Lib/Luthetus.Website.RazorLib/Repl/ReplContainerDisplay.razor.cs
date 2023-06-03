using Fluxor.Blazor.Web.Components;
using Luthetus.TextEditor.RazorLib.Group;
using Luthetus.TextEditor.RazorLib;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Website.RazorLib.Store.ReplCase;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Website.RazorLib.Pages;
using Luthetus.Ide.ClassLib.Store.EditorCase;

namespace Luthetus.Website.RazorLib.Repl;

public partial class ReplContainerDisplay : FluxorComponent
{
    [Inject]
    private IState<ReplState> ReplStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [CascadingParameter]
    private ReplPage.ViewKind ViewKind { get; set; }

    private static readonly TreeViewStateKey ReplFolderExplorerTreeViewStateKey = TreeViewStateKey.NewTreeViewStateKey();
    private static readonly TreeViewStateKey ReplSolutionExplorerTreeViewStateKey = TreeViewStateKey.NewTreeViewStateKey();
    private static readonly TreeViewStateKey ReplSemanticExplorerTreeViewStateKey = TreeViewStateKey.NewTreeViewStateKey();

    protected override void OnInitialized()
    {
        TextEditorService.Group.Register(EditorState.EditorTextEditorGroupKey);

        base.OnInitialized();
    }
}