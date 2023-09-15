using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Ide.RazorLib.InstallationCase.Models;
using Luthetus.Common.RazorLib.Installation.Models;
using Luthetus.TextEditor.RazorLib.Installation.Models;

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
            typeof(Ide.RazorLib.InstallationCase.Models.ServiceCollectionExtensions).Assembly));
    }
}
