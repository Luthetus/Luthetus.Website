using Fluxor;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Website.RazorLib.Facts;

namespace Luthetus.Website.RazorLib.Repl.FolderExplorer;

public class ReplFolderExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IDispatcher _dispatcher;

    public ReplFolderExplorerTreeViewMouseEventHandler(
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
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

        _dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            treeViewAbsoluteFilePath.Item,
            true,
            ReplFacts.TextEditorGroupKeys.GroupKey));

        return Task.FromResult(true);
    }
}