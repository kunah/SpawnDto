using DTOs.Dto;
using Model.Model;

namespace Convertors.Convertor
{
    public static class BaseModelConvertor
    {
        public static BaseDto ToBaseDto(this BaseModel model)
        {
            BaseDto dto = new BaseDto();
            dto.Id = model.Id;
            return dto;
        }

        public static BaseModel ToModel(this BaseDto dto)
        {
            BaseModel model = new BaseModel();
            model.Id = dto.Id;
            return model;
        }
    }
}