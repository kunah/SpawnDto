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

        if (args.Length < 3)
        {
            Console.WriteLine(
                "Usage: dotnet AutoDto.dll [assemblyPath] [DTO-outputPath] [DTO-namespace] (optional)[convertorOutputPath] (optional)[convertor-namespace]");
            return;
        }

        var generator = new Generator.Generator(args[0], args[1], args[2],args.Length > 3 ? args[3] : null, args.Length > 4 ? args[4] : args[2]);
        
        generator.Run();
        
        Console.WriteLine("Done!");
        
    }
    
}