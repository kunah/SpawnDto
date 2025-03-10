using AutoDto.Test;
using AutoDto.Test.Dto;

namespace AutoDto.Test.Convertor
{
    public static class BasicModelConvertor
    {
        public static BasicModelDto ToBasicModelDto(this BasicModel model)
        {
            BasicModelDto dto = new BasicModelDto();
            dto.Test = model.Test;
            dto.AnotherModel = model.AnotherModel;
            dto.Test2 = model.Test2;
            dto.DtoName = model.Name;
            return dto;
        }

        public static BasicModel ToModel(this BasicModelDto dto)
        {
            BasicModel model = new BasicModel();
            model.Test = dto.Test;
            model.AnotherModel = dto.AnotherModel;
            model.Test2 = dto.Test2;
            model.Name = dto.DtoName;
            return model;
        }

        public static FullBasic ToFullBasic(this BasicModel model)
        {
            FullBasic dto = new FullBasic();
            dto.Test = BasicConvertor.ToDto(model.Test);
            dto.AnotherModel = model.AnotherModel;
            dto.DtoName = model.Name;
            return dto;
        }

        public static BasicModel ToModel(this FullBasic dto)
        {
            BasicModel model = new BasicModel();
            model.Test = BasicConvertor.ToField(dto.Test);
            model.AnotherModel = dto.AnotherModel;
            model.Name = dto.DtoName;
            return model;
        }
    }
}