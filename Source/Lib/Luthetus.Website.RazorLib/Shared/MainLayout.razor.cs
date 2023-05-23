using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.Options;
using Luthetus.Website.RazorLib.Settings;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.Shared;

public partial class MainLayout : FluxorLayout, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    protected override void OnInitialized()
    {
        AppOptionsService.AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private void OpenSettingsDialogOnClick()
    {
        DialogService.RegisterDialogRecord(new DialogRecord(
            SettingsDisplay.SettingsDialogKey,
            "Settings",
            typeof(SettingsDisplay),
            null,
            null)
        {
            IsResizable = true
        });
    }

    public void Dispose()
    {
        AppOptionsService.AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}