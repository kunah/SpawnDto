namespace SpawnDto.Core.Convertors;

public interface IConvertor<TFrom, TTo>
{
    static abstract TTo ToDto(TFrom field);
    static abstract TFrom ToField(TTo dto);
}