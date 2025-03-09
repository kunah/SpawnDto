using System.Runtime.CompilerServices;
using AutoDto.Interfaces;

namespace AutoDto.Attributes;

/// <summary>
///
/// </summary>
/// <typeparam name="T">Generic type for specifying target data type</typeparam>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class AutoDtoAttribute : Attribute
{

    private readonly string? _customName; // name of the property in the dto
    private readonly string[] _dtos; // in which dtos it's contained
    public string[] Dtos => _dtos;
    private readonly Type? _targetType; // dto Type
    public Type? TargetType => _targetType;
    
    private readonly Type? _convertor; // convertor for this field
    public Type? Convertor => _convertor;
    private readonly string? _methodName; // converting method
    public string? MethodName => _methodName;

    public AutoDtoAttribute() : this(null, []) {}

    public AutoDtoAttribute(string customName) : this(customName, []) {}

    public AutoDtoAttribute(string[] dtos) : this(null, dtos) {}

    public AutoDtoAttribute(string? customName, string[] dtoNames) : this(customName, dtoNames, null) {}

    public AutoDtoAttribute(string? customName, Type? targetType = null, Type? convertor = null, string? methodName = null)
        : this(customName, [], targetType, convertor, methodName) {}
    
    public AutoDtoAttribute(string[] dtoNames, Type? targetType = null,
        Type? convertor = null, string? methodName = null)
        : this(null, dtoNames, targetType, convertor, methodName) { }
    
    public AutoDtoAttribute(string? customName, string[] dtoNames, Type? targetType = null,
        Type? convertor = null, string? methodName = null)
    {
        if (targetType != null && convertor == null)
            throw new ArgumentNullException(nameof(convertor),"Convertor can't be null when targetType is used!");
        if (convertor != null)
        {
            if (methodName == null)
                throw new ArgumentNullException(nameof(methodName), "Method name can't be null when convertor is used!");    
            var method = convertor.GetMethod(methodName);
            if(method == null || !method.IsStatic || method.IsPrivate)
                throw new ArgumentNullException(nameof(method), "Can't access method");
            if(targetType != null && method.ReturnType != targetType)
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
        _methodName = methodName;
    }
        
}