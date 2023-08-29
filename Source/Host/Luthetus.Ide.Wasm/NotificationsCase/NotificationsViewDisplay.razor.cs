using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.Wasm.NotificationsCase;

public partial class NotificationsViewDisplay : FluxorComponent
{
    [Inject]
    private IState<NotificationRecordsCollection> NotificationRecordsCollectionWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private NotificationsViewKind _chosenNotificationsViewKind = NotificationsViewKind.Notifications;

    private string GetIsActiveCssClass(
        NotificationsViewKind chosenNotificationsViewKind,
        NotificationsViewKind iterationNotificationsViewKind)
    {
        return chosenNotificationsViewKind == iterationNotificationsViewKind
            ? "luth_active"
            : string.Empty;
    }

    private void Clear()
    {
        Dispatcher.Dispatch(new NotificationRecordsCollection.ClearAction());
    }

    private void ClearRead()
    {
        Dispatcher.Dispatch(new NotificationRecordsCollection.ClearReadAction());
    }

    private void ClearDeleted()
    {
        Dispatcher.Dispatch(new NotificationRecordsCollection.ClearDeletedAction());
    }

    private void ClearArchived()
    {
        Dispatcher.Dispatch(new NotificationRecordsCollection.ClearArchivedAction());
    }
}
