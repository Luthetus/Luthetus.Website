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
    public static readonly string C_SHARP_PROJECT_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/BlazorWasmApp.csproj";
    
    public static readonly string C_SHARP_PROJECT_CONTENTS = @"<Project Sdk=""Microsoft.NET.Sdk.BlazorWebAssembly"">

  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly"" Version=""7.0.5"" />
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly.DevServer"" Version=""7.0.5"" PrivateAssets=""all"" />
  </ItemGroup>

</Project>
";
}
