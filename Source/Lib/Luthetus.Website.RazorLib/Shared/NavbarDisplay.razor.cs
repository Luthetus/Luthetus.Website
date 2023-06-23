using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Fluxor;
using Luthetus.Common.RazorLib.Store.DropdownCase;
using Microsoft.AspNetCore.Components.Web;

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
    private const int NAVBAR_HEIGHT_IN_PIXELS = 64;
    private const int NAVBAR_Z_INDEX = 9999;

    private string NavbarIsCollapsedCssClassString => _navbarIsCollapsed
        ? "luth_web_navbar_collapsed"
        : string.Empty;

    private bool _navbarIsCollapsed = true;

    private void ShowInputFileDialogOnClick()
    {
        Dispatcher.Dispatch(new EditorState.ShowInputFileAction());
    }

    private void SetNavbarIsCollapsedToTrue(MouseEventArgs mouseEventArgs)
    {
        _navbarIsCollapsed = true;
    }
}