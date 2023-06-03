using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Options;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Website.RazorLib.Pages;

public partial class ReplPage : ComponentBase
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    /// <summary>
    /// TODO: Measure the true height of the title div? FontSize doesn't necessary result in the same height value.
    /// <br/><br/>
    /// The height insurance in pixels is to reduce likely hood that the height for the text node is larger than that of the div itself. If a more accurate measurement of the div's height is taken then perhaps this constant would not be necessary.
    /// </summary>
    private const int HEIGHT_INSURANCE_IN_PIXELS = 20;
    
    private const int HEIGHT_OF_TITLE_DIV_BORDER_BOTTOM_IN_PIXELS = 4;

    private int ActiveRadioButtonIndex = 0;

    /// <summary>
    /// TODO: Measure the true height of the title div? FontSize doesn't necessary result in the same height value.
    /// </summary>
    private int GetHeightOfTitleDivWithoutBorder(AppOptionsState appOptionsState)
    {
        var fontSizeInPixels = appOptionsState.Options.FontSizeInPixels ??
            AppOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS;

        return fontSizeInPixels + HEIGHT_INSURANCE_IN_PIXELS;
    }

    private string GetTitleCssStyleString(AppOptionsState appOptionsState)
    {
        var heightOfTitleDivWithoutBorderCssValue = GetHeightOfTitleDivWithoutBorder(appOptionsState)
            .ToCssValue();

        return $"height: {heightOfTitleDivWithoutBorderCssValue}px;" +
            $" border-bottom: {HEIGHT_OF_TITLE_DIV_BORDER_BOTTOM_IN_PIXELS}px solid var(--luth_primary-border-color);";
    }
    
    private string GetBodyCssStyleString(AppOptionsState appOptionsState)
    {
        var totalHeightOfTitleDivCssValue = (GetHeightOfTitleDivWithoutBorder(appOptionsState) +
            HEIGHT_OF_TITLE_DIV_BORDER_BOTTOM_IN_PIXELS)
            .ToCssValue();

        return $"height: calc(100% - {totalHeightOfTitleDivCssValue}px);";
    }

    private string GetIsActiveCssClass(
        int radioButtonIndex,
        int renderBatchActiveRadioButtonIndex)
    {
        return radioButtonIndex == renderBatchActiveRadioButtonIndex
            ? "luth_active"
            : string.Empty;
    }
    
    /// <summary>
    /// I feel as though all logic regarding choosing a view is pretty hacky and gross. I've been programming for like 9 hours straight? I don't know I'm exhausted and sorry for how this is written. But, I still think I'm writing productive code so I'll try and continue working.
    /// </summary>
    private ViewKind CascadeChosenView(
        int renderBatchActiveRadioButtonIndex)
    {
        if (renderBatchActiveRadioButtonIndex == 0)
            return ViewKind.Solution;
        else if (renderBatchActiveRadioButtonIndex == 1)
            return ViewKind.Folder;
        else
            return ViewKind.Semantic;
    }

    private void SetActiveRadioButtonIndexOnClick(
        int radioButtonIndex)
    {
        ActiveRadioButtonIndex = radioButtonIndex;
    }

    public enum ViewKind
    {
        Solution,
        Folder,
        Semantic,
    }
}