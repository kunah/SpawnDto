namespace AutoDto.Interfaces;

public interface IDtoFieldConvertor<TDtoType, TFieldType> 
    where TDtoType : class
    where TFieldType : class
{
    // TODO: Add culture info
    TDtoType ToDto(TFieldType field);
    TFieldType ToField(TDtoType dto);
}