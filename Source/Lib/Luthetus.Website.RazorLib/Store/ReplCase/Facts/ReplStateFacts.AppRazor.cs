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
    public static readonly string APP_RAZOR_FILE_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/App.razor";
    
    public static readonly string APP_RAZOR_FILE_CONTENTS = @"<Router AppAssembly=""@typeof(App).Assembly"">
    <Found Context=""routeData"">
        <RouteView RouteData=""@routeData"" DefaultLayout=""@typeof(MainLayout)"" />
        <FocusOnNavigate RouteData=""@routeData"" Selector=""h1"" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout=""@typeof(MainLayout)"">
            <p role=""alert"">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
";
}
