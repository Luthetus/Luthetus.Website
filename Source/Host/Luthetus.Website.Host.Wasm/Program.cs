var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddLuthetusWebsiteServices();

builder.Services.AddSingleton<CommonQueuedHostedService>();
builder.Services.AddSingleton<TextEditorQueuedHostedService>();
builder.Services.AddSingleton<CompilerServiceQueuedHostedService>();

builder.Services.AddSingleton<ICommonBackgroundTaskQueue, CommonBackgroundTaskQueueSingleThreaded>();
builder.Services.AddSingleton<ITextEditorBackgroundTaskQueue, TextEditorBackgroundTaskQueueSingleThreaded>();
builder.Services.AddSingleton<ICompilerServiceBackgroundTaskQueue, CompilerServiceBackgroundTaskQueueSingleThreaded>();

await builder.Build().RunAsync();
