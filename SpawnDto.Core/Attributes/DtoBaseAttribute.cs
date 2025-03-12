namespace SpawnDto.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class DtoBaseAttribute : Attribute
{

    private readonly Type? _baseType;
    public Type? BaseType => _baseType;

    public DtoBaseAttribute(string name) : this(Helpers.CreateType(name)) {}

    public DtoBaseAttribute(Type? baseType)
    {
        _baseType = baseType;
    }
    
}