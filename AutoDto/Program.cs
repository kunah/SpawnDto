namespace AutoDto;

public class Program
{

    public static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        // if (args.Length == 0)
        // {
        //     Console.WriteLine("Usage: AutoDto.exe <assembly>");
        //     return;
        // }

        if (args.Length < 2)
        {
            Console.WriteLine(
                "Usage: dotnet AutoDto.dll [assemblyPath] [Dto-outputPath] (optional)[convertorOutputPath]");
            return;
        }

        var generator = new Generator.Generator(args[0], args[1], args.Length > 2 ? args[2] : null);
        
        generator.Run();
        
        Console.WriteLine("Done!");
        
    }
    
}