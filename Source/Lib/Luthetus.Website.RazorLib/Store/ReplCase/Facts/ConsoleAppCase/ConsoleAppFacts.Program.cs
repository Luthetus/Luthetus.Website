namespace Luthetus.Website.RazorLib.Store.ReplCase.Facts.ConsoleAppCase;

public partial class ConsoleAppFacts
{
    public static readonly string PROGRAM_CS_FILE_ABSOLUTE_FILE_PATH = @"/ConsoleApp/Program.cs";

    public static readonly string PROGRAM_CS_FILE_CONTENTS = @"// Hello World! program
namespace HelloWorld
{
    class Hello 
    {         
        static void Main(string[] args)
        {
            System.Console.WriteLine(""Hello World!"");
        }
    }
}";
}