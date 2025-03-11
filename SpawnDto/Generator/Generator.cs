using System.Reflection;
using System.Runtime.CompilerServices;
using SpawnDto.Attributes;

namespace SpawnDto.Generator;

public class Generator
{
    private readonly string? _assemblyPath;
    private readonly string _outputPath;
    private readonly string _dtoNamespace;
    private readonly string? _convertorOutputPath;
    private readonly string? _convertorNamespace;

    public Generator(string? assemblyPath, string outputPath, string dtoNamespace, string? convertorOutputPath, string? convertorNamespace = null)
    {
        _assemblyPath = assemblyPath;
        _outputPath = outputPath;
        _dtoNamespace = dtoNamespace;
        _convertorOutputPath = convertorOutputPath;
        _convertorNamespace = convertorNamespace;
    }

    public void Run()
    {
        var assembly = GetAssembly();

        var types = assembly.GetTypes();
        
        var classes = types.Where(type => type.GetCustomAttribute(typeof(GenerateDtoAttribute)) != null).ToArray();

        Console.WriteLine(Directory.GetCurrentDirectory());
        
        ClassGenerator generator = new ClassGenerator();
        foreach (var cl in classes)
        {
            Console.WriteLine(cl.Name);
            generator.GenerateClass(cl, "", _outputPath, _dtoNamespace, _convertorOutputPath ?? _outputPath, _convertorNamespace ?? _dtoNamespace);
        }
    }

    private Assembly GetAssembly()
    {
        if(_assemblyPath == null)
            return Assembly.GetExecutingAssembly();
        return Assembly.LoadFrom(_assemblyPath);
    }
    
}