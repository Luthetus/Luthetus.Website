using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Fluxor;

namespace Luthetus.Website.RazorLib.Shared;

public partial class NavbarDisplay : ComponentBase
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public Action OpenSettingsDialogAction { get; set; } = null!;

    private const double ICON_SIZE_MULTIPLIER = 1.5;

    private void ShowInputFileDialogOnClick()
    {
        Dispatcher.Dispatch(new EditorState.ShowInputFileAction());
    }
}