using System.Reflection;
using System.Runtime.CompilerServices;

namespace AutoDto.Attributes;

/// <summary>
///
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class AutoDtoAttribute : Attribute
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

    public AutoDtoAttribute() : this(null, []) {}

    public AutoDtoAttribute(string customName) : this(customName, []) {}

    public AutoDtoAttribute(string[] dtos) : this(null, dtos) {}

    public AutoDtoAttribute(string? customName, string[] dtoNames) : this(customName, dtoNames, null) {}

    public AutoDtoAttribute(string? customName, Type? targetType = null, Type? convertor = null,
        string? toDtoMethod = null, string? fromDtoMethod = null)
        : this(customName, [], targetType, convertor, toDtoMethod, fromDtoMethod) {}
    
    public AutoDtoAttribute(string[] dtoNames, Type? targetType = null,
        Type? convertor = null, string? toDtoMethod = null, string? fromDtoMethod = null)
        : this(null, dtoNames, targetType, convertor, toDtoMethod, fromDtoMethod) { }
    
    public AutoDtoAttribute(string? customName, string[] dtoNames, Type? targetType = null,
        Type? convertor = null, string? toDtoMethod = null, string? fromDtoMethod = null)
    {
        if (targetType != null && convertor == null)
            throw new ArgumentNullException(nameof(convertor),"Convertor can't be null when targetType is used!");
        if (convertor != null)
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
    }
        
}