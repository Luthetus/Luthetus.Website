namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts.BlazorWasmAppCase;

public partial class BlazorWasmAppFacts
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