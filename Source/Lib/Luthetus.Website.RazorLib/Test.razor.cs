using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Options;
using Luthetus.Common.RazorLib.Resize;
using Luthetus.Common.RazorLib.StateHasChangedBoundaryCase;
using Luthetus.Common.RazorLib.Store.AppOptionsCase;
using Luthetus.Common.RazorLib.Store.DragCase;
using Luthetus.Common.RazorLib.Store.PanelCase;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Luthetus.TextEditor.RazorLib;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib;

public partial class Test : LayoutComponentBase, IDisposable
{
    [Inject]
    private IState<DragRegistry> DragRegistryWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsRegistry> AppOptionsRegistryWrap { get; set; } = null!;
    [Inject]
    private IState<PanelsRegistry> PanelsCollectionWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

    private string UnselectableClassCss => DragRegistryWrap.Value.ShouldDisplay
        ? "balc_unselectable"
        : string.Empty;

    private bool _previousDragStateWrapShouldDisplay;
    private ElementDimensions _bodyElementDimensions = new();
    private StateHasChangedBoundary _bodyAndFooterStateHasChangedBoundaryComponent = null!;
    private int _count;
    private string[] _fileBag = Array.Empty<string>();
    private string _absolutePathString = string.Empty;
    private bool _isDirectory;

    protected override void OnInitialized()
    {
        DragRegistryWrap.StateChanged += DragStateWrapOnStateChanged;
        AppOptionsRegistryWrap.StateChanged += AppOptionsStateWrapOnStateChanged;
        ContinuousBackgroundTaskWorker.Queue.ExecutingBackgroundTaskChanged += Queue_ExecutingBackgroundTaskChanged;

        var bodyHeight = _bodyElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

        bodyHeight.DimensionUnits.AddRange(new[]
        {
        new DimensionUnit
        {
            Value = 78,
            DimensionUnitKind = DimensionUnitKind.Percentage
        },
        new DimensionUnit
        {
            Value = ResizableRow.RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
            DimensionUnitKind = DimensionUnitKind.Pixels,
            DimensionOperatorKind = DimensionOperatorKind.Subtract
        },
        new DimensionUnit
        {
            Value = SizeFacts.Ide.Header.Height.Value / 2,
            DimensionUnitKind = SizeFacts.Ide.Header.Height.DimensionUnitKind,
            DimensionOperatorKind = DimensionOperatorKind.Subtract
        }
    });

        base.OnInitialized();
    }

    private async void Queue_ExecutingBackgroundTaskChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await TextEditorService.Options.SetFromLocalStorageAsync();
            await AppOptionsService.SetFromLocalStorageAsync();

            if (File.Exists("C:\\Users\\hunte\\Repos\\Demos\\BlazorCrudApp\\BlazorCrudApp.sln"))
            {
                var absolutePath = new AbsolutePath(
                    "C:\\Users\\hunte\\Repos\\Demos\\BlazorCrudApp\\BlazorCrudApp.sln",
                    false,
                    EnvironmentProvider);

                Dispatcher.Dispatch(new DotNetSolutionState.SetDotNetSolutionTask(
                    absolutePath,
                    DotNetSolutionSync));
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (_previousDragStateWrapShouldDisplay != DragRegistryWrap.Value.ShouldDisplay)
        {
            _previousDragStateWrapShouldDisplay = DragRegistryWrap.Value.ShouldDisplay;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task ReRenderAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void IncrementCount()
    {
        _count++;
    }
    
    private async Task ReadDirectoryAsync()
    {
        _fileBag = (await FileSystemProvider.Directory.EnumerateFileSystemEntriesAsync(
            _absolutePathString))
            .ToArray();
    }

    public void Dispose()
    {
        DragRegistryWrap.StateChanged -= DragStateWrapOnStateChanged;
        AppOptionsRegistryWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
        ContinuousBackgroundTaskWorker.Queue.ExecutingBackgroundTaskChanged -= Queue_ExecutingBackgroundTaskChanged;
    }
}