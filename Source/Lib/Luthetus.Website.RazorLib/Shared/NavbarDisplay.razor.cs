using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Fluxor;
using Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Website.RazorLib.Shared;

public partial class NavbarDisplay : ComponentBase
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private INotificationService NotificationService { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IState<ReplState> ReplStateWrap { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public Action OpenSettingsDialogAction { get; set; } = null!;

    private const double ICON_SIZE_MULTIPLIER = 1.5;

    private void BuildAndRunOnClick()
    {
        var replState = ReplStateWrap.Value;

        foreach (var replFile in replState.Files)
        {
            var text = replFile.Data;

            var isDirectory = 
                replFile.AbsoluteFilePathString.EndsWith(EnvironmentProvider.DirectorySeparatorChar) ||
                replFile.AbsoluteFilePathString.EndsWith(EnvironmentProvider.AltDirectorySeparatorChar);

            if (isDirectory)
                continue;

            var absoluteFilePath = new AbsoluteFilePath(
                replFile.AbsoluteFilePathString,
                isDirectory,
                EnvironmentProvider);

            var informativeNotifaction = new NotificationRecord(
                    NotificationKey.NewNotificationKey(),
                    $"TextEditorModel: {absoluteFilePath.FilenameWithExtension}",
                    LuthetusCommonComponentRenderers.InformativeNotificationRendererType,
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(IErrorNotificationRendererType.Message),
                            text
                        }
                    },
                    TimeSpan.FromSeconds(10),
                    null);

            NotificationService.RegisterNotificationRecord(informativeNotifaction);
        }
    }
}