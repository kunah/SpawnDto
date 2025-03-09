using System.ComponentModel.DataAnnotations;
using AutoDto.Attributes;

namespace AutoDto.Test;

public static class BasicConvertor
{
    public static int ToInt(string value)
    {
        return 100;
    }
}

[GenerateDto("BasicModelDto", "FullBasic")]
public class BasicModel
{
    [AutoDto(["FullBasic"], typeof(int), typeof(BasicConvertor), nameof(BasicConvertor.ToInt))]
    [AutoDto(["BasicModelDto"])]
    public string Test { get; set; } = "test";
    
    [AutoDto(["BasicModelDto"])]
    public float Test2 = 1.1f;

    [AutoDto] 
    public string? Name = string.Empty;
    
    [AutoDto, Required]
    public AutoDtoAttribute? AnotherModel { get; set; }
}