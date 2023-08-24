using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.Wasm.NotificationsCase;

public partial class NotificationsInteractiveIconDisplay : FluxorComponent
{
    [Inject]
    private IState<NotificationRecordsCollection> NotificationRecordsCollectionWrap { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

    private readonly DialogRecord NotificationsViewDisplayDialogRecord = new(
        DialogKey.NewDialogKey(),
        "Notifications",
        typeof(NotificationsViewDisplay),
        null,
        null)
    {
        IsResizable = true
    };

    private void ShowNotificationsViewDisplayOnClick()
    {
        DialogService.RegisterDialogRecord(NotificationsViewDisplayDialogRecord);
    }
}