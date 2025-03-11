using CommandLine;

namespace AutoDto;

public class Program
{

    public class Options
    {
        [Option('a', "assemblyPath", Required = true, HelpText = "Set the assembly path.")]
        public string AssemblyPath { get; set; }
        [Option('d', "dto", Required = true, HelpText = "Where to output the DTOs.")]
        public string DtoOutputPath { get; set; }
        [Option('n', "dtoNamespace", Required = true, HelpText = "Namespace to output the DTOs.")]
        public string DtoNamespace { get; set; }
        [Option( "convertorOutputPath", Required = false, HelpText = "Where to output the convertors.")]
        public string? ConvertorOutputPath { get; set; }
        [Option("convertorNamespace", Required = false, HelpText = "Namespace to output the convertors.")]
        public string? ConvertorNamespace { get; set; }
    }
    
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                var generator = new Generator.Generator(o.AssemblyPath, o.DtoOutputPath, o.DtoNamespace,
                    o.ConvertorOutputPath ?? o.DtoOutputPath, o.ConvertorNamespace ?? o.DtoNamespace);
                generator.Run();
            });

        Console.WriteLine("Done!");
        
    }
    
}