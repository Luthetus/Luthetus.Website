﻿using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Fluxor;
using Luthetus.Website.RazorLib.Facts;

namespace Luthetus.Website.RazorLib.Repl.SolutionExplorer;

public class ReplSolutionExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IDispatcher _dispatcher;

    public ReplSolutionExplorerTreeViewMouseEventHandler(
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
            is not TreeViewNamespacePath treeViewNamespacePath)
        {
            return Task.FromResult(false);
        }

        if (treeViewNamespacePath.Item is null)
            return Task.FromResult(false);

        _dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            true,
            ReplFacts.TextEditorGroupKeys.GroupKey));

        return Task.FromResult(true);
    }
}