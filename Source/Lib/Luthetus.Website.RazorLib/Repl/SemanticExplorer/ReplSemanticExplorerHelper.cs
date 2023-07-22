using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.BoundClassDefinitionNodeCase;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.ISyntaxCase;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.SyntaxTokenTextCase;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax;
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
        else if (activeNode is TreeViewBoundClassDefinitionNode treeViewBoundClassDefinitionNode &&
                 treeViewBoundClassDefinitionNode.Item is not null)
        {
            textSpan = treeViewBoundClassDefinitionNode.Item.TypeClauseToken.TextSpan;
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