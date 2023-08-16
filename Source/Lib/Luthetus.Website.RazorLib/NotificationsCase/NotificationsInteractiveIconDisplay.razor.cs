using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.NotificationsCase;

public partial class NotificationsInteractiveIconDisplay : FluxorComponent
{
    [Inject]
    private IState<NotificationRecordsCollection> NotificationRecordsCollectionWrap { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
}