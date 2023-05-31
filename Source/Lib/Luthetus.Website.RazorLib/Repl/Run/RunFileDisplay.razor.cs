using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Luthetus.Website.RazorLib.Shared;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
namespace Luthetus.Website.RazorLib.Repl.Run;

public partial class RunFileDisplay : IRunFileDisplayRenderer
{
    [Parameter, EditorRequired]
    public string SourceText { get; set; } = null!;
}