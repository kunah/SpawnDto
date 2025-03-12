using System.Reflection;
using System.Reflection.Emit;

namespace SpawnDto.Core.Attributes;

internal static class Helpers
{
    
    internal static Type? CreateType(string? name)
    {
        if(name == null)
            return null;
        
        AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
        
        var typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Class | TypeAttributes.Public);
        return typeBuilder.CreateType();
    }
    
}