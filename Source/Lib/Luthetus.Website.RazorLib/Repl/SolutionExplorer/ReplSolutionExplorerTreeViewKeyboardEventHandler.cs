﻿using Luthetus.Common.RazorLib.BackgroundTaskCase;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Fluxor;
using Luthetus.TextEditor.RazorLib.Group;

namespace Luthetus.Website.RazorLib.Repl.SolutionExplorer;

public class ReplSolutionExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly TextEditorGroupKey _replTextEditorGroupKey;
    private readonly ICommonMenuOptionsFactory _commonMenuOptionsFactory;
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IDispatcher _dispatcher;
    private readonly ITreeViewService _treeViewService;
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;

    public ReplSolutionExplorerTreeViewKeyboardEventHandler(
        TextEditorGroupKey replTextEditorGroupKey,
        ICommonMenuOptionsFactory commonMenuOptionsFactory,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IDispatcher dispatcher,
        ITreeViewService treeViewService,
        ITextEditorService textEditorService,
        IBackgroundTaskQueue backgroundTaskQueue)
        : base(treeViewService)
    {
        _replTextEditorGroupKey = replTextEditorGroupKey;
        _commonMenuOptionsFactory = commonMenuOptionsFactory;
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _dispatcher = dispatcher;
        _treeViewService = treeViewService;
        _textEditorService = textEditorService;
        _backgroundTaskQueue = backgroundTaskQueue;
    }

    public override async Task<bool> OnKeyDownAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.KeyboardEventArgs is null)
            return false;

        _ = await base.OnKeyDownAsync(treeViewCommandParameter);

        switch (treeViewCommandParameter.KeyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                await InvokeOpenInEditorAsync(
                    treeViewCommandParameter,
                    true);
                return true;
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                await InvokeOpenInEditorAsync(
                    treeViewCommandParameter,
                    false);
                return true;
        }

        return false;
    }

    private async Task InvokeOpenInEditorAsync(
        ITreeViewCommandParameter treeViewCommandParameter,
        bool shouldSetFocusToEditor)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null ||
            activeNode is not TreeViewNamespacePath treeViewNamespacePath ||
            treeViewNamespacePath.Item is null)
        {
            return;
        }

        await EditorState.OpenInEditorAsync(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            shouldSetFocusToEditor,
            _dispatcher,
            _textEditorService,
            _luthetusIdeComponentRenderers,
            _fileSystemProvider,
            _backgroundTaskQueue,
            _replTextEditorGroupKey);
    }
}