using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Fluxor;
using Luthetus.TextEditor.RazorLib.Group;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.ISyntaxCase;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.TextEditor.RazorLib.Semantics;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.SyntaxTokenTextCase;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.BoundClassDeclarationNodeCase;

namespace Luthetus.Website.RazorLib.Repl.SemanticExplorer;

public class ReplSemanticExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly TextEditorGroupKey _replTextEditorGroupKey;
    private readonly ITextEditorService _textEditorService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDispatcher _dispatcher;

    public ReplSemanticExplorerTreeViewKeyboardEventHandler(
        TextEditorGroupKey replTextEditorGroupKey,
        ITextEditorService textEditorService,
        IEnvironmentProvider environmentProvider,
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _replTextEditorGroupKey = replTextEditorGroupKey;
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

        TextEditorTextSpan textSpan;

        if (activeNode is TreeViewISyntax treeViewISyntax)
        {
            if (treeViewISyntax.Item is ISyntaxNode syntaxNode)
            {
                var childToken = syntaxNode.Children.FirstOrDefault(x => x is ISyntaxToken);

                if (childToken is null)
                    return Task.CompletedTask;

                textSpan = ((ISyntaxToken)childToken).TextSpan;
            }
            else if (treeViewISyntax.Item is ISyntaxToken syntaxToken)
            {
                textSpan = syntaxToken.TextSpan;
            }
            else
            {
                return Task.CompletedTask;
            }
        }
        else if (activeNode is TreeViewSyntaxTokenText treeViewSyntaxTokenText &&
                 treeViewSyntaxTokenText.Item is not null)
        {
            textSpan = treeViewSyntaxTokenText.Item.TextSpan;
        }
        else if (activeNode is TreeViewBoundClassDeclarationNode treeViewBoundClassDeclarationNode &&
                 treeViewBoundClassDeclarationNode.Item is not null)
        {
            textSpan = treeViewBoundClassDeclarationNode.Item.IdentifierToken.TextSpan;
        }
        else
        {
            return Task.CompletedTask;
        }

        var model = _textEditorService.Model.FindOrDefaultByResourceUri(
            textSpan.ResourceUri);

        if (model is null)
            return Task.CompletedTask;

        var viewModels = _textEditorService.Model.GetViewModelsOrEmpty(model.ModelKey);

        if (!viewModels.Any())
            return Task.CompletedTask;

        var viewModel = viewModels[0];

        var rowInformation = model.FindRowInformation(
            textSpan.StartingIndexInclusive);

        viewModel.PrimaryCursor.IndexCoordinates =
            (rowInformation.rowIndex,
                textSpan.StartingIndexInclusive - rowInformation.rowStartPositionIndex);

        _dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            new AbsoluteFilePath(
                textSpan.ResourceUri.Value,
                false,
                _environmentProvider),
            shouldSetFocusToEditor,
            _replTextEditorGroupKey));

        return Task.CompletedTask;
    }
}