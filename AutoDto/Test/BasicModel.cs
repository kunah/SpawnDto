using System.ComponentModel.DataAnnotations;
using AutoDto.Attributes;

namespace AutoDto.Test;

public static class BasicConvertor
{
    public static int ToDto(string field)
    {
        return field.GetHashCode();
    }

    public static string ToField(int dto)
    {
        throw new NotImplementedException();
    }
}

[GenerateDto("BasicModelDto", "FullBasic")]
public class BasicModel
{
    [AutoDto(["FullBasic"], typeof(int), typeof(BasicConvertor), nameof(BasicConvertor.ToDto), nameof(BasicConvertor.ToField))]
    [AutoDto(["BasicModelDto"])]
    public string Test { get; set; } = "test";
    
    [AutoDto(["BasicModelDto"])]
    public float Test2 = 1.1f;

    [AutoDto] 
    public string? Name = string.Empty;
    
    [AutoDto, Required]
    public AutoDtoAttribute? AnotherModel { get; set; }
}