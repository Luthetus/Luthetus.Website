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
using Luthetus.Website.RazorLib.Facts;

namespace Luthetus.Website.RazorLib.Repl.SemanticExplorer;

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