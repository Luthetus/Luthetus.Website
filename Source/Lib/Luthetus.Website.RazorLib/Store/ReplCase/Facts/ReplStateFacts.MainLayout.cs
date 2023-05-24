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
    public static readonly string MAIN_LAYOUT_RAZOR_FILE_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/MainLayout.razor";
    
    public static readonly string MAIN_LAYOUT_RAZOR_FILE_CONTENTS = @"@inherits LayoutComponentBase

<main>
    @Body
</main>
";
}
