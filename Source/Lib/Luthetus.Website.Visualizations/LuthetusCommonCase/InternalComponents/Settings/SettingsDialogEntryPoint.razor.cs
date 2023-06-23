using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.RenderTracker;

namespace Luthetus.Ide.Photino.TestApps.TestAppLuthetusCommonCase.InternalComponents.Settings;

public partial class SettingsDialogEntryPoint : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private static readonly DialogRecord _dialogRecord = new(
        DialogKey.NewDialogKey(),
        "Settings",
        typeof(SettingsDisplay),
        null,
        null)
    {
        IsResizable = true
    };

    public void DispatchRegisterDialogRecordAction()
    {
        Dispatcher.Dispatch(
            new DialogRecordsCollection.RegisterAction(
                _dialogRecord));
    }
}