using System.Collections.Generic;

namespace NF.Tools.ProtocolFlow.Message
{
    public class MessageDic
    {
        public Dictionary<string, MessagePack> _dic = new Dictionary<string, MessagePack>();

        public MessageDic(IEnumerable<MessageInfo> infos)
        {
            foreach (MessageInfo mi in infos)
            {
                this.Add(mi);
            }
        }

        private void Add(MessageInfo mi)
        {
            MessagePack mp = null;
            string key = mi.MessageName;
            if (!this._dic.TryGetValue(key, out mp))
            {
                mp = new MessagePack();
                this._dic.Add(key, mp);
            }

            switch (mi.MessageType)
            {
                case E_MESSAGE.Request:
                    mp.Request = mi;
                    break;
                case E_MESSAGE.Response:
                    mp.Response = mi;
                    break;
                default:
                    break;
            }
        }
    }
}