using SpawnDto.Core.Attributes;
using SpawnDto.Core.Convertors;

namespace Model.Model;

public class BasicConvertor : IConvertor<string, int>
{
    public static int ToDto(string field)
    {
        return field.GetHashCode();
    }

    public static string ToField(int dto)
    {
        return dto.ToString() ?? string.Empty;
    }
}

[SpawnDto("BasicModelDto", "FullBasic")]
[DtoBase("BaseDto")]
public class BasicModel
{
    [DtoProperty(["FullBasic"], typeof(int), typeof(BasicConvertor))]
    [DtoProperty(["BasicModelDto"])]
    public string? Test { get; set; } = "test";
    
    [DtoProperty(["BasicModelDto"])]
    public float Test2 = 1.1f;

    [DtoProperty("DtoName")] 
    public string? Name = string.Empty;
    
    [DtoProperty]
    public string Always = string.Empty;

    [DtoProperty("SecondDto", true )]
    public SecondModel model;
    
    public string Never = string.Empty;
}