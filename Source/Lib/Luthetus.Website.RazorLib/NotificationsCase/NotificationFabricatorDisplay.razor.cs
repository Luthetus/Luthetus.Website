using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.NotificationsCase;

public partial class NotificationFabricatorDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;

    private string _title = string.Empty;
    private string _message = string.Empty;
    private int _lifespanInSeconds = 5;
    private bool _isError = false;

    private void RegisterNotificationOnClick()
    {
        var rendererType = _isError
            ? LuthetusCommonComponentRenderers.ErrorNotificationRendererType
            : LuthetusCommonComponentRenderers.InformativeNotificationRendererType;

        var parameterName = _isError
            ? nameof(IErrorNotificationRendererType.Message)
            : nameof(IInformativeNotificationRendererType.Message);

        var cssClassString = _isError
            ? IErrorNotificationRendererType.CSS_CLASS_STRING
            : null;

        var notificationRecord = new NotificationRecord(
            NotificationKey.NewNotificationKey(),
            _title,
            rendererType,
            new Dictionary<string, object?>
            {
                {
                    parameterName,
                    _message
                },
            },
            TimeSpan.FromSeconds(_lifespanInSeconds),
            false,
            cssClassString);

        var registerAction = new NotificationRecordsCollection.RegisterAction(notificationRecord);

        Dispatcher.Dispatch(registerAction);
    }
}