using SpawnDto.Core.Attributes;

namespace Model.Model;

[GenerateDto("SecondDto")]
public class SecondModel
{
    [SpawnDto]
    public int Id { get; set; }
}