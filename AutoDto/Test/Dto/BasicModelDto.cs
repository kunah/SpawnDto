using System;
using AutoDto.Attributes;

namespace AutoDto.Test.Dto
{
    public partial class BasicModelDto
    {
        public String? Test { get; set; } = "test";
        public AutoDtoAttribute AnotherModel { get; set; }
        public Single Test2 { get; set; } = 1.1F;
        public String DtoName { get; set; } = "";
    }
}