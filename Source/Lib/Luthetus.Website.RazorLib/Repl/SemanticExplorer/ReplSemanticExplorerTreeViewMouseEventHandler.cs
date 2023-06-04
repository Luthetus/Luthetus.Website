using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Fluxor;
using Luthetus.TextEditor.RazorLib.Group;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.ISyntaxCase;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.BoundClassDeclarationNodeCase;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.SyntaxTokenTextCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Website.RazorLib.Repl.SemanticExplorer;

public class ReplSemanticExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly TextEditorGroupKey _replTextEditorGroupKey;
    private readonly ITextEditorService _textEditorService;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IDispatcher _dispatcher;

    public ReplSemanticExplorerTreeViewMouseEventHandler(
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

    public override Task<bool> OnDoubleClickAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _ = base.OnDoubleClickAsync(treeViewCommandParameter);

        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is null)
            return Task.FromResult(false);

        TextEditorTextSpan textSpan;

        if (activeNode is TreeViewISyntax treeViewISyntax)
        {
            if (treeViewISyntax.Item is ISyntaxNode syntaxNode)
            {
                var childToken = syntaxNode.Children.FirstOrDefault(x => x is ISyntaxToken);

                if (childToken is null)
                    return Task.FromResult(false);

                textSpan = ((ISyntaxToken)childToken).TextSpan;
            }
            else if (treeViewISyntax.Item is ISyntaxToken syntaxToken)
            {
                textSpan = syntaxToken.TextSpan;
            }
            else
            {
                return Task.FromResult(false);
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
            return Task.FromResult(false);
        }

        var model = _textEditorService.Model.FindOrDefaultByResourceUri(
            textSpan.ResourceUri);

        if (model is null)
            return Task.FromResult(false);

        var viewModels = _textEditorService.Model.GetViewModelsOrEmpty(model.ModelKey);

        if (!viewModels.Any())
            return Task.FromResult(false);

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
            true,
            _replTextEditorGroupKey));

        return Task.FromResult(true);
    }
}