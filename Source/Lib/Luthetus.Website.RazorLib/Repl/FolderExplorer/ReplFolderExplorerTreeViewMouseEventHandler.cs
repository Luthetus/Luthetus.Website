using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Fluxor;
using Luthetus.TextEditor.RazorLib.Group;

namespace Luthetus.Website.RazorLib.Repl.FolderExplorer;

public class ReplFolderExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly TextEditorGroupKey _replTextEditorGroupKey;
    private readonly IDispatcher _dispatcher;

    public ReplFolderExplorerTreeViewMouseEventHandler(
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
            is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)
        {
            return Task.FromResult(false);
        }

        if (treeViewAbsoluteFilePath.Item is null)
            return Task.FromResult(false);

        _dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            treeViewAbsoluteFilePath.Item,
            true,
            _replTextEditorGroupKey));

        return Task.FromResult(true);
    }
}