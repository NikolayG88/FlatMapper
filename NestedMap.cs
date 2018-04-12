using System;

namespace FlatMapper
{
    //TODO: Add default value property and implement it in the mapper
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class NestedMap : Attribute
    {
        public string Path { get; set; }

        public string TargetProperty { get; set; }
    }

}
