using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Luthetus.Website.Host.Wasm;
using Luthetus.Website.RazorLib;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.TextEditor.RazorLib.HostedServiceCase;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddLuthetusWebsiteServices();

builder.Services.AddSingleton<CommonQueuedHostedService>();
builder.Services.AddSingleton<TextEditorQueuedHostedService>();

builder.Services.AddSingleton<ICommonBackgroundTaskQueue, CommonBackgroundTaskQueueSingleThreaded>();
builder.Services.AddSingleton<ITextEditorBackgroundTaskQueue, TextEditorBackgroundTaskQueueSingleThreaded>();

await builder.Build().RunAsync();
