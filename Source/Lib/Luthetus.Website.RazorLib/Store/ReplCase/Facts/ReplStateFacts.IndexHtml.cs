namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts;

public partial class ReplStateFacts
{
    public static readonly string INDEX_HTML_FILE_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/wwwroot/index.html";

    public static readonly string INDEX_HTML_FILE_CONTENTS = @"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""utf-8"" />
    <title>BlazorWasmApp</title>
    <base href=""/"" />
    <link href=""css/app.css"" rel=""stylesheet"" />
    
    <!-- If you add any scoped CSS files, uncomment the following to load them
    <link href=""BlazorWasmApp.styles.css"" rel=""stylesheet"" /> -->
</head>

<body>
    <div id=""app"">Loading...</div>

    <div id=""blazor-error-ui"">
        An unhandled error has occurred.
        <a href="""" class=""reload"">Reload</a>
        <a class=""dismiss"">🗙</a>
    </div>
    <script src=""_framework/blazor.webassembly.js""></script>
</body>

</html>
";
}