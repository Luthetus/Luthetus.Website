using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.NotificationsCase;

public partial class NotificationsViewDisplay : FluxorComponent
{
    [Inject]
    private IState<NotificationRecordsCollection> NotificationRecordsCollectionWrap { get; set; } = null!;

    private NotificationsViewKind _chosenNotificationsViewKind = NotificationsViewKind.Notifications;

    private string GetIsActiveCssClass(
        NotificationsViewKind chosenNotificationsViewKind,
        NotificationsViewKind iterationNotificationsViewKind)
    {
        return chosenNotificationsViewKind == iterationNotificationsViewKind
            ? "luth_active"
            : string.Empty;
    }
}
