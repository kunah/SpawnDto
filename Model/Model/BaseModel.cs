using SpawnDto.Core.Attributes;

namespace Model.Model;

[SpawnDto("BaseDto")]
public class BaseModel
{
    [DtoProperty]
    public int Id;
}