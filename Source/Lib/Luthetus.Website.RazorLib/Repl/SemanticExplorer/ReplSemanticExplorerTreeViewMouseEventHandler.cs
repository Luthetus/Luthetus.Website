using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Fluxor;
using Luthetus.TextEditor.RazorLib.Group;

namespace Luthetus.Website.RazorLib.Repl.SemanticExplorer;

public class ReplSemanticExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly TextEditorGroupKey _replTextEditorGroupKey;
    private readonly IDispatcher _dispatcher;

    public ReplSemanticExplorerTreeViewMouseEventHandler(
        TextEditorGroupKey replTextEditorGroupKey,
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _replTextEditorGroupKey = replTextEditorGroupKey;
        _dispatcher = dispatcher;
    }

    public override Task<bool> OnDoubleClickAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _ = base.OnDoubleClickAsync(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode
            is not TreeViewNamespacePath treeViewNamespacePath)
        {
            return Task.FromResult(false);
        }

        if (treeViewNamespacePath.Item is null)
            return Task.FromResult(false);

        _dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            true,
            _replTextEditorGroupKey));

        return Task.FromResult(true);
    }
}