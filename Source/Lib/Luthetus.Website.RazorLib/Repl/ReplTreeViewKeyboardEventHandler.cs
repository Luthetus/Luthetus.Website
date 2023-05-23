using Luthetus.Common.RazorLib.BackgroundTaskCase;
using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Fluxor;

namespace Luthetus.Website.RazorLib.Repl;

public class ReplTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IDispatcher _dispatcher;
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;

    public ReplTreeViewKeyboardEventHandler(
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IDispatcher dispatcher,
        ITreeViewService treeViewService,
        ITextEditorService textEditorService,
        IBackgroundTaskQueue backgroundTaskQueue)
        : base(treeViewService)
    {
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _dispatcher = dispatcher;
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
            activeNode is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePathPath ||
            treeViewAbsoluteFilePathPath.Item is null)
        {
            return;
        }

        await EditorState.OpenInEditorAsync(
            treeViewAbsoluteFilePathPath.Item,
            shouldSetFocusToEditor,
            _dispatcher,
            _textEditorService,
            _luthetusIdeComponentRenderers,
            _fileSystemProvider,
            _backgroundTaskQueue);
    }
}