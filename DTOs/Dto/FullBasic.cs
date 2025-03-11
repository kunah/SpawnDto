using System;

namespace DTOs.Dto
{
    public partial class FullBasic
    {
        public Int32 Test { get; set; } = -2040035567;
        public String DtoName { get; set; } = "";
        public String? Always { get; set; } = "";
        public SecondDto? model { get; set; }
    }
}