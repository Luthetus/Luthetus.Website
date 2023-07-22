using Luthetus.Website.RazorLib.Store.ReplCase;
using Luthetus.Website.RazorLib.Facts;
using Luthetus.Website.RazorLib.ViewCase;
using Fluxor.Blazor.Web.Components;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.TextEditor.RazorLib;

namespace Luthetus.Website.RazorLib.Repl;

public partial class ReplContainerDisplay : FluxorComponent
{
    [Inject]
    private IState<ReplState> ReplStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [CascadingParameter]
    private ViewKind ViewKind { get; set; }

    protected override void OnInitialized()
    {
        TextEditorService.Group.Register(ReplFacts.TextEditorGroupKeys.GroupKey);

        base.OnInitialized();
    }
}