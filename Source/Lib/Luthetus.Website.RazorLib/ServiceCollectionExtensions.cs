using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib;
using Luthetus.Ide.RazorLib;
using Luthetus.TextEditor.RazorLib;

namespace Luthetus.Website.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusWebsiteServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation)
    {
        services.AddLuthetusIdeRazorLibServices(hostingInformation);

        return services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonOptions).Assembly,
            typeof(LuthetusTextEditorOptions).Assembly,
            typeof(Ide.ClassLib.InstallationCase.ServiceCollectionExtensions).Assembly));
    }
}
