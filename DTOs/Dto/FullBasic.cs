using System;

namespace DTOs.Dto
{
    public partial class FullBasic
    {
        public Int32 Test { get; set; } = 722944099;
        public String DtoName { get; set; } = "";
        public String? Always { get; set; } = "";
        public SecondDto? model { get; set; }
    }
}