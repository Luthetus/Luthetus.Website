using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.BoundClassDeclarationNodeCase;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.ISyntaxCase;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.SyntaxTokenTextCase;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Website.RazorLib.Repl.SemanticExplorer;

public static class ReplSemanticExplorerHelper
{
    public static void OpenInEditor(
        bool shouldSetFocusToEditor,
        TreeViewNoType? activeNode,
        ITextEditorService textEditorService,
        IDispatcher dispatcher,
        IEnvironmentProvider environmentProvider)
    {
        TextEditorTextSpan textSpan;

        if (activeNode is TreeViewISyntax treeViewISyntax)
        {
            if (treeViewISyntax.Item is ISyntaxNode syntaxNode)
            {
                var childToken = syntaxNode.Children.FirstOrDefault(x => x is ISyntaxToken);

                if (childToken is null)
                    return;

                textSpan = ((ISyntaxToken)childToken).TextSpan;
            }
            else if (treeViewISyntax.Item is ISyntaxToken syntaxToken)
            {
                textSpan = syntaxToken.TextSpan;
            }
            else
            {
                return;
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
            return;
        }

        var model = textEditorService.Model.FindOrDefaultByResourceUri(
            textSpan.ResourceUri);

        if (model is null)
            return;

        var viewModels = textEditorService.Model.GetViewModelsOrEmpty(model.ModelKey);

        if (!viewModels.Any())
        {
            dispatcher.Dispatch(new EditorState.OpenInEditorAction(
                new AbsoluteFilePath(model.ResourceUri.Value, false, environmentProvider),
                true,
                Facts.ReplFacts.TextEditorGroupKeys.GroupKey));

            // TODO: Do not hackily create a ViewModel, and get a reference to it here
            viewModels = textEditorService.Model.GetViewModelsOrEmpty(model.ModelKey);

            if (!viewModels.Any())
                return;
        }

        var viewModel = viewModels[0];

        var rowInformation = model.FindRowInformation(
            textSpan.StartingIndexInclusive);

        viewModel.PrimaryCursor.IndexCoordinates =
            (rowInformation.rowIndex,
                textSpan.StartingIndexInclusive - rowInformation.rowStartPositionIndex);

        dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            new AbsoluteFilePath(
                textSpan.ResourceUri.Value,
                false,
                environmentProvider),
            shouldSetFocusToEditor,
            Facts.ReplFacts.TextEditorGroupKeys.GroupKey));

        return;
    }
}
