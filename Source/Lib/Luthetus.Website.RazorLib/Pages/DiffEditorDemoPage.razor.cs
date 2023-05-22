using Luthetus.Website.RazorLib.Store.WellKnownModelKindCase;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Analysis.CSharp.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Css.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Css.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.FSharp.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Html.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.JavaScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Json.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Json.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Razor.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.TypeScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Decoration;
using Luthetus.TextEditor.RazorLib.Diff;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.ViewModel;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.Pages;

public partial class DiffEditorDemoPage : SingleComponentPage
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IState<WellKnownModelKindState> WellKnownModelKindStateWrap { get; set; } = null!;

    private static readonly TextEditorDiffKey DiffEditorDemoDiffModelKey = TextEditorDiffKey.NewTextEditorDiffKey();

    private static readonly TextEditorModelKey DiffEditorDemoBeforeModelKey = TextEditorModelKey.NewTextEditorModelKey();
    private static readonly TextEditorViewModelKey DiffEditorDemoBeforeViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    private static readonly TextEditorModelKey DiffEditorDemoAfterModelKey = TextEditorModelKey.NewTextEditorModelKey();
    private static readonly TextEditorViewModelKey DiffEditorDemoAfterViewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

    private bool _disposed;

    protected override void OnInitialized()
    {
        WellKnownModelKindStateWrap.StateChanged += WellKnownModelKindStateWrapOnStateChanged;

        TextEditorService.Model.RegisterTemplated(
            DiffEditorDemoBeforeModelKey,
            WellKnownModelKind.CSharp,
            "textEditorDemoBefore.txt",
            DateTime.UtcNow,
            "C#",
            "ABCDEFK");
        //TEXT_EDITOR_DEMO_INITIAL_CONTENT);

        TextEditorService.ViewModel.Register(
            DiffEditorDemoBeforeViewModelKey,
            DiffEditorDemoBeforeModelKey);

        TextEditorService.Model.RegisterTemplated(
            DiffEditorDemoAfterModelKey,
            WellKnownModelKind.CSharp,
            "textEditorDemoAfter.txt",
            DateTime.UtcNow,
            "C#",
            "BHDEFCK");
        //TEXT_EDITOR_DEMO_INITIAL_CONTENT);

        TextEditorService.ViewModel.Register(
            DiffEditorDemoAfterViewModelKey,
            DiffEditorDemoAfterModelKey);

        TextEditorService.Diff.Register(
            DiffEditorDemoDiffModelKey,
            DiffEditorDemoBeforeViewModelKey,
            DiffEditorDemoAfterViewModelKey);

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            WellKnownModelKindStateWrapOnStateChanged(null, EventArgs.Empty);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async void WellKnownModelKindStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        // Capture the mutable references locally first thing 

        var localWellKnownModelState = WellKnownModelKindStateWrap.Value;

        var beforeModel = TextEditorService
            .Model.FindOrDefault(DiffEditorDemoBeforeModelKey);

        var afterModel = TextEditorService
            .Model.FindOrDefault(DiffEditorDemoAfterModelKey);

        ITextEditorLexer? lexer = null;
        IDecorationMapper? decorationMapper = null;

        switch (localWellKnownModelState.WellKnownModelKind)
        {
            case WellKnownModelKind.CSharp:
                lexer = new TextEditorCSharpLexer();
                decorationMapper = new GenericDecorationMapper();
                break;
            case WellKnownModelKind.Html:
                lexer = new TextEditorHtmlLexer();
                decorationMapper = new TextEditorHtmlDecorationMapper();
                break;
            case WellKnownModelKind.Css:
                lexer = new TextEditorCssLexer();
                decorationMapper = new TextEditorCssDecorationMapper();
                break;
            case WellKnownModelKind.Json:
                lexer = new TextEditorJsonLexer();
                decorationMapper = new TextEditorJsonDecorationMapper();
                break;
            case WellKnownModelKind.FSharp:
                lexer = new TextEditorFSharpLexer();
                decorationMapper = new GenericDecorationMapper();
                break;
            case WellKnownModelKind.Razor:
                lexer = new TextEditorRazorLexer();
                decorationMapper = new TextEditorHtmlDecorationMapper();
                break;
            case WellKnownModelKind.JavaScript:
                lexer = new TextEditorJavaScriptLexer();
                decorationMapper = new GenericDecorationMapper();
                break;
            case WellKnownModelKind.TypeScript:
                lexer = new TextEditorTypeScriptLexer();
                decorationMapper = new GenericDecorationMapper();
                break;
        }

        if (beforeModel is not null)
        {
            beforeModel.SetLexer(lexer);
            beforeModel.SetDecorationMapper(decorationMapper);

            await beforeModel.ApplySyntaxHighlightingAsync();
        }

        if (afterModel is not null)
        {
            afterModel.SetLexer(lexer);
            afterModel.SetDecorationMapper(decorationMapper);

            await afterModel.ApplySyntaxHighlightingAsync();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            WellKnownModelKindStateWrap.StateChanged -= WellKnownModelKindStateWrapOnStateChanged;
        }

        _disposed = true;

        base.Dispose(disposing);
    }
}