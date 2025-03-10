using System;
using AutoDto.Attributes;

namespace AutoDto.Test.Dto
{
    public partial class FullBasic
    {
        public Int32 Test { get; set; } = -1933234920;
        public AutoDtoAttribute AnotherModel { get; set; }
        public String DtoName { get; set; } = "";
    }
}