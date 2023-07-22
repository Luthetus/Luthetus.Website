using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.TextEditor.RazorLib;

namespace Luthetus.Website.RazorLib.Repl.SemanticExplorer;

public class ReplSemanticExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly ITextEditorService _textEditorService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDispatcher _dispatcher;

    public ReplSemanticExplorerTreeViewKeyboardEventHandler(
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

    private Task InvokeOpenInEditorAsync(
        ITreeViewCommandParameter treeViewCommandParameter,
        bool shouldSetFocusToEditor)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null)
            return Task.CompletedTask;

        ReplSemanticExplorerHelper.OpenInEditor(
            shouldSetFocusToEditor,
            activeNode,
            _textEditorService,
            _dispatcher,
            _environmentProvider);

        return Task.CompletedTask;
    }
}