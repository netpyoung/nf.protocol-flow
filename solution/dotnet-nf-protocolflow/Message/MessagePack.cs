using DotLiquid;

namespace NF.Tools.ProtocolFlow.Message
{
    public class MessagePack : Drop
    {
        public MessageInfo Request { get; set; }
        public MessageInfo Response { get; set; }
    }
}