namespace AutoDto.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class GenerateDtoAttribute : Attribute
{

    private readonly string[] _names;
    
    public string[] Names => _names;
    
    public GenerateDtoAttribute(params string[] names)
    {
        _names = names;
    }

}