using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace SpawnDto.Core.Attributes;

/// <summary>
///
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class SpawnDtoAttribute : Attribute
{

    private readonly string? _customName; // name of the property in the dto
    public string? CustomName => _customName;
    private readonly string[] _dtos; // in which dtos it's contained
    public string[] Dtos => _dtos;
    private readonly Type? _targetType; // dto Type
    public Type? TargetType => _targetType;
    
    private readonly Type? _convertor; // convertor for this field
    public Type? Convertor => _convertor;
    
    private readonly string? _toDtoMethod; // converting method
    private readonly string? _fromDtoMethod; // converting method
    public string? ToDtoMethod => _toDtoMethod;
    public string? FromDtoMethod => _fromDtoMethod;

    private readonly bool _willBeGenerated;
    public bool WillBeGenerated => _willBeGenerated;

    public SpawnDtoAttribute() : this(null, []) {}

    public SpawnDtoAttribute(string customName) : this(customName, []) {}

    public SpawnDtoAttribute(string[] dtos) : this(null, dtos) {}

    public SpawnDtoAttribute(string? customName, string[] dtoNames) : this(customName, dtoNames, null) {}
    
    public SpawnDtoAttribute(Type? convertor, string? toDtoMethod, string? fromDtoMethod)
        : this(targetType: null, convertor, toDtoMethod, fromDtoMethod) {}

    public SpawnDtoAttribute(string? targetTypeString, bool willBeGenerated) : this(targetTypeString, null, null, null, willBeGenerated) {}
    
    public SpawnDtoAttribute(string? targetTypeString, Type? convertor, string? toDtoMethod, string? fromDtoMethod, bool willBeGenerated = false)
        : this(null, [], CreateType(targetTypeString), convertor, toDtoMethod, fromDtoMethod, willBeGenerated) {}
    
    public SpawnDtoAttribute(Type? targetType, Type? convertor, string? toDtoMethod, string? fromDtoMethod, bool willBeGenerated = false)
        : this(null, [], targetType, convertor, toDtoMethod, fromDtoMethod, willBeGenerated) {}

    public SpawnDtoAttribute(string? customName, string? targetTypeString = null, Type? convertor = null,
        string? toDtoMethod = null, string? fromDtoMethod = null, bool willBeGenerated = false)
        : this(customName, [], CreateType(targetTypeString), convertor, toDtoMethod, fromDtoMethod, willBeGenerated) {}
        
    public SpawnDtoAttribute(string? customName, Type? targetType = null, Type? convertor = null,
        string? toDtoMethod = null, string? fromDtoMethod = null, bool willBeGenerated = false)
        : this(customName, [], targetType, convertor, toDtoMethod, fromDtoMethod, willBeGenerated) {}
    
    public SpawnDtoAttribute(string[] dtoNames, string? targetTypeString = null,
        Type? convertor = null, string? toDtoMethod = null, string? fromDtoMethod = null, bool willBeGenerated = false)
        : this(null, dtoNames, CreateType(targetTypeString), convertor, toDtoMethod, fromDtoMethod, willBeGenerated) { }
    
    public SpawnDtoAttribute(string[] dtoNames, Type? targetType = null,
        Type? convertor = null, string? toDtoMethod = null, string? fromDtoMethod = null, bool willBeGenerated = false)
        : this(null, dtoNames, targetType, convertor, toDtoMethod, fromDtoMethod, willBeGenerated) { }

    
    public SpawnDtoAttribute(string? customName, string[] dtoNames, Type? targetType = null,
        Type? convertor = null, string? toDtoMethod = null, string? fromDtoMethod = null, bool willBeGenerated = false)
    {
        if (targetType != null && convertor == null && !willBeGenerated)
            throw new ArgumentNullException(nameof(convertor),"Convertor can't be null when targetType is used!");
        
        if (convertor != null && !willBeGenerated)
        {
            if (toDtoMethod == null)
                throw new ArgumentNullException(nameof(toDtoMethod), "Method name can't be null when convertor is used!");    
            if (fromDtoMethod == null)
                throw new ArgumentNullException(nameof(fromDtoMethod), "Method name can't be null when convertor is used!");    
            var methodToDto = convertor.GetMethod(toDtoMethod);
            var methodFromDto = convertor.GetMethod(fromDtoMethod);
            if(methodToDto == null || !methodToDto.IsStatic || methodToDto.IsPrivate)
                throw new ArgumentNullException(nameof(toDtoMethod), "Can't access method");
            if(methodFromDto == null || !methodFromDto.IsStatic || methodFromDto.IsPrivate)
                throw new ArgumentNullException(nameof(fromDtoMethod), "Can't access method");
            if(targetType != null && methodToDto.ReturnType != targetType &&
               methodFromDto.GetParameters().Length != 1 && methodFromDto.GetParameters()[0].ParameterType != targetType)
                throw new ArgumentNullException(nameof(targetType),"Method has to have the same return type!");
            _toDtoMethod = toDtoMethod;
            _fromDtoMethod = fromDtoMethod;
        }
        // if (targetType != null && convertor != null)
        // {
        //     if(!targetType.IsAssignableFrom(convertor.Value.toDtoConvertor.Method.ReturnType))
        //         throw new ArgumentException($"Target type of {nameof(convertor.Value.toDtoConvertor)} must be of type {targetType.Name} or it's subclass");
        //
        //     var parameters = convertor.Value.fromDtoConvertor.Method.GetParameters();
        //     
        //     if(parameters.Length != 1)
        //         throw new ArgumentException($"Method {nameof(convertor.Value.fromDtoConvertor)} must have one parameter");
        //     if(!targetType.IsAssignableFrom(parameters[0].ParameterType))
        //         throw new ArgumentException($"Target type of {nameof(convertor.Value.fromDtoConvertor)} must be of type {targetType.Name} or it's subclass");
        // }
        
        _customName = customName;
        _dtos = dtoNames;
        _targetType = targetType;
        _convertor = convertor;
        _willBeGenerated = willBeGenerated;
    }

    private static Type? CreateType(string? name)
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