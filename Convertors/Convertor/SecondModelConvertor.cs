using DTOs.Dto;
using Model.Model;

namespace Convertors.Convertor
{
    public static class SecondModelConvertor
    {
        public static SecondDto ToSecondDto(this SecondModel model)
        {
            SecondDto dto = new SecondDto();
            dto.Id = model.Id;
            return dto;
        }

        public static SecondModel ToModel(this SecondDto dto)
        {
            SecondModel model = new SecondModel();
            model.Id = dto.Id;
            return model;
        }
    }
}