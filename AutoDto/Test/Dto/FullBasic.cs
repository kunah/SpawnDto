using System;

namespace AutoDto.Test.Dto
{
    public partial class FullBasic
    {
        public System.Int32? Test { get; set; }
        public AutoDto.Attributes.AutoDtoAttribute AnotherModel { get; set; }
        public System.String Name { get; set; }
    }
}