using System;

namespace DTOs.Dto
{
    public partial class BasicModelDto : BaseDto
    {
        public String Test { get; set; } = "test";
        public Single Test2 { get; set; } = 1.1F;
        public String DtoName { get; set; } = "";
        public String? Always { get; set; } = "";
        public SecondDto? model { get; set; }
    }
}