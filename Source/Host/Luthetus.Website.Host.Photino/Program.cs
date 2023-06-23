﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Ide.ClassLib.CompilerServices.HostedServiceCase;
using Luthetus.Ide.ClassLib.FileSystem.HostedServiceCase;
using Luthetus.Ide.RazorLib;
using Luthetus.TextEditor.RazorLib.HostedServiceCase;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;

namespace Luthetus.Website.Host.Photino
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

            appBuilder.Services
                .AddLogging();

            appBuilder.Services.AddLuthetusIdeRazorLibServices(true);

            // The code:
            //     builder.Services.AddHostedService<QueuedHostedService>();
            //
            // is not working for the Photino Blazor app.
            // So manual starting of the service is done.
            appBuilder.Services.AddSingleton<CommonQueuedHostedService>();
            appBuilder.Services.AddSingleton<TextEditorQueuedHostedService>();
            appBuilder.Services.AddSingleton<FileSystemQueuedHostedService>();
            appBuilder.Services.AddSingleton<CompilerServiceQueuedHostedService>();

            appBuilder.RootComponents.Add<App>("app");

            var app = appBuilder.Build();

            var backgroundTasksCancellationTokenSource = new CancellationTokenSource();

            var commonQueuedHostedService = app.Services.GetRequiredService<CommonQueuedHostedService>();
            var textEditorQueuedHostedService = app.Services.GetRequiredService<TextEditorQueuedHostedService>();
            var fileSystemQueuedHostedService = app.Services.GetRequiredService<FileSystemQueuedHostedService>();
            var compilerServiceQueuedHostedService = app.Services.GetRequiredService<CompilerServiceQueuedHostedService>();

            var cancellationToken = backgroundTasksCancellationTokenSource.Token;

            _ = Task.Run(async () => await commonQueuedHostedService.StartAsync(cancellationToken));
            _ = Task.Run(async () => await textEditorQueuedHostedService.StartAsync(cancellationToken));
            _ = Task.Run(async () => await fileSystemQueuedHostedService.StartAsync(cancellationToken));
            _ = Task.Run(async () => await compilerServiceQueuedHostedService.StartAsync(cancellationToken));

            // customize window
            app.MainWindow
                .SetIconFile("favicon.ico")
                .SetTitle("Luthetus IDE")
                .SetDevToolsEnabled(true)
                .SetContextMenuEnabled(true)
                .SetUseOsDefaultSize(false)
                .SetSize(2600, 1800)
                .SetLeft(50)
                .SetTop(100);

            AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
            {
                backgroundTasksCancellationTokenSource.Cancel();
                app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
            };

            app.Run();

        }
    }
}
