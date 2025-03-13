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
    
    private const string _toDtoMethod = "ToDto"; // converting method
    private const string _fromDtoMethod = "ToField"; // converting method
    public string ToDtoMethod => _toDtoMethod;
    public string FromDtoMethod => _fromDtoMethod;

    private readonly bool _willBeGenerated;
    public bool WillBeGenerated => _willBeGenerated;

    public SpawnDtoAttribute() : this(null, []) {}

    public SpawnDtoAttribute(string customName) : this(customName, []) {}

    public SpawnDtoAttribute(string[] dtos) : this(null, dtos) {}

    public SpawnDtoAttribute(string? customName, string[] dtoNames) : this(customName, dtoNames, null) {}
    
    public SpawnDtoAttribute(Type? convertor)
        : this(targetType: null, convertor) {}

    public SpawnDtoAttribute(string? targetTypeString, bool willBeGenerated) : this(targetTypeString, null, willBeGenerated) {}
    
    public SpawnDtoAttribute(string? targetTypeString, Type? convertor, bool willBeGenerated = false)
        : this(null, [], Helpers.CreateType(targetTypeString), convertor, willBeGenerated) {}
    
    public SpawnDtoAttribute(Type? targetType, Type? convertor, bool willBeGenerated = false)
        : this(null, [], targetType, convertor, willBeGenerated) {}

    public SpawnDtoAttribute(string? customName, string? targetTypeString = null, Type? convertor = null, bool willBeGenerated = false)
        : this(customName, [], Helpers.CreateType(targetTypeString), convertor, willBeGenerated) {}
        
    public SpawnDtoAttribute(string? customName, Type? targetType = null, Type? convertor = null, bool willBeGenerated = false)
        : this(customName, [], targetType, convertor, willBeGenerated) {}
    
    public SpawnDtoAttribute(string[] dtoNames, string? targetTypeString = null,
        Type? convertor = null, bool willBeGenerated = false)
        : this(null, dtoNames, Helpers.CreateType(targetTypeString), convertor, willBeGenerated) { }
    
    public SpawnDtoAttribute(string[] dtoNames, Type? targetType = null,
        Type? convertor = null, bool willBeGenerated = false)
        : this(null, dtoNames, targetType, convertor, willBeGenerated) { }

    
    public SpawnDtoAttribute(string? customName, string[] dtoNames, Type? targetType = null,
        Type? convertor = null, bool willBeGenerated = false)
    {
        if (targetType != null && convertor == null && !willBeGenerated)
            throw new ArgumentNullException(nameof(convertor),"Convertor can't be null when targetType is used!");
        
        if (convertor != null && !willBeGenerated)
        {
            var methodToDto = convertor.GetMethod(_toDtoMethod);
            var methodFromDto = convertor.GetMethod(_fromDtoMethod);
            if(methodToDto == null || !methodToDto.IsStatic || methodToDto.IsPrivate)
                throw new ArgumentNullException(nameof(_toDtoMethod), "Can't access method");
            if(methodFromDto == null || !methodFromDto.IsStatic || methodFromDto.IsPrivate)
                throw new ArgumentNullException(nameof(_fromDtoMethod), "Can't access method");
            if(targetType != null && methodToDto.ReturnType != targetType &&
               methodFromDto.GetParameters().Length != 1 && methodFromDto.GetParameters()[0].ParameterType != targetType)
                throw new ArgumentNullException(nameof(targetType),"Method has to have the same return type!");
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
    
}