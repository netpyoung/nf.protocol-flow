using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotLiquid;

namespace NF.Tools.ProtocolFlow.Message
{
    public class MessageInfo : Drop
    {
        public MessageInfo(Type type)
        {
            this.Type = type;
            PropertyInfo[] properties =
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
            this.Arguments = new List<Argument>();
            this.Arguments.AddRange(
                properties.ToList().Select(p => new Argument {Type = p.PropertyType, Name = p.Name}));
        }

        public E_MESSAGE MessageType { get; set; }
        public string MessageName { get; set; }
        public string Name => this.Type.Name;
        public Type Type { get; }
        public List<Argument> Arguments { get; }
    }
}