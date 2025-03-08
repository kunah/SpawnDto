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

        var generator = new Generator.Generator(args.Length > 0 ? args[0] : null);
        
        generator.Run();
        
        Console.WriteLine("Done!");
        
    }
    
}