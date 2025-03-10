using System.Reflection;
using System.Runtime.CompilerServices;
using AutoDto.Attributes;

namespace AutoDto.Generator;

public class Generator
{
    private readonly string? _assemblyPath;
    private readonly string _outputPath;
    private readonly string? _convertorOutputPath;

    public Generator(string? assemblyPath, string outputPath, string? convertorOutputPath)
    {
        _assemblyPath = assemblyPath;
        _outputPath = outputPath;
        _convertorOutputPath = convertorOutputPath;
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
            generator.GenerateClass(cl, "", _outputPath, _convertorOutputPath ?? _outputPath);
        }
    }

    private Assembly GetAssembly()
    {
        if(_assemblyPath == null)
            return Assembly.GetExecutingAssembly();
        return Assembly.LoadFrom(_assemblyPath);
    }
    
}