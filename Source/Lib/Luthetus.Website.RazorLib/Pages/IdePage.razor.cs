using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.Pages;

public partial class IdePage : ComponentBase, IDisposable
{
    [Inject]
    private DotNetSolutionCompilerService DotNetSolutionCompilerService { get; set; } = null!;

    protected override void OnInitialized()
    {
        DotNetSolutionCompilerService.ModelRegistered += OnEventRequiresReRender; 
        DotNetSolutionCompilerService.ModelDisposed += OnEventRequiresReRender; 
        DotNetSolutionCompilerService.ModelParsed += OnEventRequiresReRender; 

        base.OnInitialized();
    }

    private async void OnEventRequiresReRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        DotNetSolutionCompilerService.ModelRegistered -= OnEventRequiresReRender;
        DotNetSolutionCompilerService.ModelDisposed -= OnEventRequiresReRender;
        DotNetSolutionCompilerService.ModelParsed -= OnEventRequiresReRender;
    }
}