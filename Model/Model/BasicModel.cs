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

[GenerateDto("BasicModelDto", "FullBasic")]
[DtoBase("BaseDto")]
public class BasicModel
{
    [SpawnDto(["FullBasic"], typeof(int), typeof(BasicConvertor))]
    [SpawnDto(["BasicModelDto"])]
    public string? Test { get; set; } = "test";
    
    [SpawnDto(["BasicModelDto"])]
    public float Test2 = 1.1f;

    [SpawnDto("DtoName")] 
    public string? Name = string.Empty;
    
    [SpawnDto]
    public string Always = string.Empty;

    [SpawnDto("SecondDto", true )]
    public SecondModel model;
    
    public string Never = string.Empty;
}