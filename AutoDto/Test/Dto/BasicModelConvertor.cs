namespace AutoDto.Test.Dto
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

        public static FullBasic ToFullBasic(this BasicModel model)
        {
            FullBasic dto = new FullBasic();
            dto.Test = BasicConvertor.ToDto(model.Test);
            dto.AnotherModel = model.AnotherModel;
            dto.DtoName = model.Name;
            return dto;
        }
    }
}