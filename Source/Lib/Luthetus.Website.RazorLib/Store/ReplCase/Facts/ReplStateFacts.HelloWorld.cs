namespace Luthetus.Website.RazorLib.Store.InMemoryFileSystemCase;

public partial class ReplStateFacts
{
    public static readonly string HELLO_WORLD_CS_FILE_ABSOLUTE_FILE_PATH = @"/BlazorWasmApp/HelloWorld.cs";
    
    public static readonly string HELLO_WORLD_CS_FILE_CONTENTS = @"// Hello World! program
namespace HelloWorld
{
    class Hello {         
        static void Main(string[] args)
        {
            System.Console.WriteLine(""Hello World!"");
        }
    }
}";
}
