namespace Luthetus.Website.RazorLib.Shared;

public partial class NavbarDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, EditorRequired]
    public Action OpenSettingsDialogAction { get; set; } = null!;

    private const double ICON_SIZE_MULTIPLIER = 1.5;
    private const int NAVBAR_HEIGHT_IN_PIXELS = 64;
    private const int NAVBAR_Z_INDEX = 9999;

    private string NavbarIsCollapsedCssClassString => _navbarIsCollapsed
        ? "luth_web_navbar_collapsed"
        : string.Empty;

    private bool _navbarIsCollapsed = true;

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += NavigationManager_LocationChanged;

        base.OnInitialized();
    }

    private async void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs locationChangedEventArgs)
    {
        await InvokeAsync(StateHasChanged);
    }

    private void ShowInputFileDialogOnClick()
    {
        Dispatcher.Dispatch(new EditorState.ShowInputFileAction());
    }

    private void SetNavbarIsCollapsedToTrue(MouseEventArgs mouseEventArgs)
    {
        _navbarIsCollapsed = true;
    }

    /// <summary>
    /// TODO: Don't specify the href twice. Once for the a tag and then once for this method invocation.
    /// </summary>
    private string GetLinkIsActiveCssClassString(string href)
    {
        if (NavigationManager.Uri.Replace(NavigationManager.BaseUri, string.Empty).Contains(href))
            return "luth_active";

        return string.Empty;
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
    }
}