using System;
using DotLiquid;

namespace NF.Tools.ProtocolFlow
{
    public class Argument : Drop
    {
        public Type Type { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{this.Type} {this.Name}";
        }

        public string AssignName()
        {
            return string.Format("{0} = {0}", this.Name);
        }
    }
}