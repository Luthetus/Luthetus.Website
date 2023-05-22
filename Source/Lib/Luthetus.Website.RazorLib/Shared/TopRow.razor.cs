using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.Shared;

public partial class TopRow : ComponentBase
{
    [Parameter, EditorRequired]
    public Action OpenSettingsDialogOnClick { get; set; } = null!;
}