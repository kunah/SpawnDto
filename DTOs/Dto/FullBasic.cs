using System;

namespace DTOs.Dto
{
    public partial class FullBasic : BaseDto
    {
        public Int32 Test { get; set; } = 11334725;
        public String DtoName { get; set; } = "";
        public String? Always { get; set; } = "";
        public SecondDto? model { get; set; }
    }
}