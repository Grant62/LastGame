using System;

namespace Services.ExcelTool
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BinaryTableAttribute : Attribute
    {
        public Type DataType { get; set; }
    }
}