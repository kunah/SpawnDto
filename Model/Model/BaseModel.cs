using SpawnDto.Core.Attributes;

namespace Model.Model;

[GenerateDto("BaseDto")]
public class BaseModel
{
    [SpawnDto]
    public int Id;
}