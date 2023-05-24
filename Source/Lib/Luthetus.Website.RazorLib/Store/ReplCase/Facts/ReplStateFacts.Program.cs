using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Resize;
using Luthetus.Ide.ClassLib.DotNet;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Website.RazorLib.Repl.FileSystem;
using System.Collections.Immutable;

namespace Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

public partial class ReplStateFacts
{
    public static readonly string PROGRAM_CS_FILE_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/Program.cs";
    
    public static readonly string PROGRAM_CS_FILE_CONTENTS = @"using BlazorWasmApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>(""#app"");
builder.RootComponents.Add<HeadOutlet>(""head::after"");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
";
}
