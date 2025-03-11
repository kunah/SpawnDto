using SpawnDto.Attributes;

namespace Model.Model;

public static class BasicConvertor
{
    public static int ToDto(string field)
    {
        return field.GetHashCode();
    }

    public static string ToField(int? dto)
    {
        return dto.ToString() ?? string.Empty;
    }
}

[GenerateDto("BasicModelDto", "FullBasic")]
public class BasicModel
{
    [SpawnDto(["FullBasic"], typeof(int), typeof(BasicConvertor), nameof(BasicConvertor.ToDto), nameof(BasicConvertor.ToField))]
    [SpawnDto(["BasicModelDto"])]
    public string? Test { get; set; } = "test";
    
    [SpawnDto(["BasicModelDto"])]
    public float Test2 = 1.1f;

    [SpawnDto("DtoName")] 
    public string? Name = string.Empty;
}