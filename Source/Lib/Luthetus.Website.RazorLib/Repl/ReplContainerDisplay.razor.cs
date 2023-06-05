using Fluxor.Blazor.Web.Components;
using Luthetus.TextEditor.RazorLib;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Website.RazorLib.Store.ReplCase;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.Website.RazorLib.Facts;
using Luthetus.Website.RazorLib.ViewCase;

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