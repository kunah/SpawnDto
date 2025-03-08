using System.ComponentModel.DataAnnotations;
using AutoDto.Attributes;

namespace AutoDto.Test;

public class AnotherModel
{
    
}

[GenerateDto("BasicModelDto", "FullBasic")]
public class BasicModel
{
    [AutoDto([ "FullBasic"], typeof(int))]
    [AutoDto(["BasicModelDto"])]
    public string Test { get; set; } = "test";
    
    [AutoDto(["BasicModelDto"])]
    public float Test2 = 1.1f;

    [AutoDto] 
    public string? Name = string.Empty;
    
    [AutoDto, Required]
    public AutoDtoAttribute? AnotherModel { get; set; }
}