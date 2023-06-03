using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Fluxor;
using Luthetus.TextEditor.RazorLib.Group;

namespace Luthetus.Website.RazorLib.Repl.SemanticExplorer;

public class ReplSemanticExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly TextEditorGroupKey _replTextEditorGroupKey;
    private readonly IDispatcher _dispatcher;

    public ReplSemanticExplorerTreeViewKeyboardEventHandler(
        TextEditorGroupKey replTextEditorGroupKey,
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _replTextEditorGroupKey = replTextEditorGroupKey;
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

        if (activeNode is null ||
            activeNode is not TreeViewNamespacePath treeViewNamespacePath ||
            treeViewNamespacePath.Item is null)
        {
            return Task.CompletedTask;
        }

        _dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            shouldSetFocusToEditor,
            _replTextEditorGroupKey));

        return Task.CompletedTask;
    }
}