using System;

namespace AutoDto.Test.Dto
{
    public partial class BasicModelDto
    {
        public System.String? Test { get; set; }
        public AutoDto.Attributes.AutoDtoAttribute AnotherModel { get; set; }
        public System.Single Test2 { get; set; }
        public System.String Name { get; set; }
    }
}