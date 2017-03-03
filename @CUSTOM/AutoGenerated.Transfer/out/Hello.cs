namespace AutoGenerated.Transfer
{
    using System;
    using NF.Network.Protocol.Interface;
    using AutoGenerated.Message;
    using AutoGenerated.Interface;

    public partial class Sender : NF.Network.Transfer.Protobuf.BaseSender,  IHello
    {
        #region IHello implementation
        public Task<RHello> Hello(QHello q)
        {
            return MessageSend<RHello> (q);
        }

        public Task<RHello> Hello(System.Int32 Q1, System.Int32 Q2)
        {
            return Hello (new QHello() { Q1 = Q1, Q2 = Q2 });
        }
        #endregion
   }
}
