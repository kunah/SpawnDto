namespace SpawnDto.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class SpawnDtoAttribute : Attribute
{

    private readonly string[] _names;
    
    public string[] Names => _names;
    
    public SpawnDtoAttribute(params string[] names)
    {
        _names = names;
    }

}