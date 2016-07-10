using System;
using VDS.RDF.Specifications;

namespace VDS.RDF.Compatibility
{
    public static class DnxCompatibilty
    {
        public static bool IsHexEncoding(String value, int index)
        {
            if (index + 2 >= value.Length) return false;
            return value[0] == '%' && TurtleSpecsHelper.IsHex(value[1]) && TurtleSpecsHelper.IsHex(value[2]);
        }
    }
}
