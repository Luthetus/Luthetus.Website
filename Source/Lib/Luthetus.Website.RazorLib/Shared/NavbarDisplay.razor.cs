using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options;

namespace Luthetus.Website.RazorLib.Shared;

public partial class NavbarDisplay : ComponentBase
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    [Parameter, EditorRequired]
    public Action OpenSettingsDialogAction { get; set; } = null!;

    private const double ICON_SIZE_MULTIPLIER = 1.5;
}