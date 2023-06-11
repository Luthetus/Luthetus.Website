using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.TextEditor.RazorLib.HostedServiceCase;
using Luthetus.Website.RazorLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddLuthetusWebsiteServices();

builder.Services.AddHostedService<CommonQueuedHostedService>();
builder.Services.AddHostedService<TextEditorQueuedHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();