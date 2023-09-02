using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;
using Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;
using Luthetus.Ide.Wasm;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;
using Luthetus.Website.RazorLib;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddLuthetusWebsiteServices(false);

var host = builder.Build();

var backgroundTasksCancellationTokenSource = new CancellationTokenSource();
var cancellationToken = backgroundTasksCancellationTokenSource.Token;

var commonQueuedHostedService = host.Services.GetRequiredService<LuthetusCommonBackgroundTaskServiceWorker>();
var textEditorQueuedHostedService = host.Services.GetRequiredService<LuthetusTextEditorTextEditorBackgroundTaskServiceWorker>();
var compilerServiceQueuedHostedService = host.Services.GetRequiredService<LuthetusTextEditorCompilerServiceBackgroundTaskServiceWorker>();
var fileSystemQueuedHostedService = host.Services.GetRequiredService<LuthetusIdeFileSystemBackgroundTaskServiceWorker>();
var terminalQueuedHostedService = host.Services.GetRequiredService<LuthetusIdeTerminalBackgroundTaskServiceWorker>();

//_ = Task.Run(async () => await commonQueuedHostedService.StartAsync(cancellationToken));
//_ = Task.Run(async () => await textEditorQueuedHostedService.StartAsync(cancellationToken));
//_ = Task.Run(async () => await compilerServiceQueuedHostedService.StartAsync(cancellationToken));
//_ = Task.Run(async () => await fileSystemQueuedHostedService.StartAsync(cancellationToken));
//_ = Task.Run(async () => await terminalQueuedHostedService.StartAsync(cancellationToken));

await host.RunAsync();