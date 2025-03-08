using System;
using AutoDto.Attributes;

namespace AutoDto.Test.Dto
{
    public partial class FullBasic
    {
        public AutoDtoAttribute AnotherModel { get; set; }
        public String Name { get; set; } = "";
    }
}