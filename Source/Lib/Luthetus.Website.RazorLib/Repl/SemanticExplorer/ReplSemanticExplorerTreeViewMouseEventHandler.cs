﻿namespace Luthetus.Website.RazorLib.Repl.SemanticExplorer;

public class ReplSemanticExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly ITextEditorService _textEditorService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDispatcher _dispatcher;

    public ReplSemanticExplorerTreeViewMouseEventHandler(
        ITextEditorService textEditorService,
        IEnvironmentProvider environmentProvider,
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _textEditorService = textEditorService;
        _environmentProvider = environmentProvider;
        _dispatcher = dispatcher;
    }

    public override Task<bool> OnDoubleClickAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _ = base.OnDoubleClickAsync(treeViewCommandParameter);

        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null)
            return Task.FromResult(false);

        ReplSemanticExplorerHelper.OpenInEditor(
            true,
            activeNode,
            _textEditorService,
            _dispatcher,
            _environmentProvider);

        return Task.FromResult(true);
    }
}