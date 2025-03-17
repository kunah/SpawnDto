using SpawnDto.Core.Attributes;

namespace Model.Model;

[SpawnDto("SecondDto")]
public class SecondModel
{
    [DtoProperty]
    public int Id { get; set; }
}