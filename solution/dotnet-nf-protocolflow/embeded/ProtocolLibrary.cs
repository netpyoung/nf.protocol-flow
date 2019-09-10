using System.Threading.Tasks;
using NF.Results;
using Google.Protobuf;
using {{ interface_namespace }};

namespace {{ protocol_namespace }}
{
	public abstract partial class BaseSender : INetworkSender<IMessage>
    {
        public abstract Task<Result<R, int>> Send<R>(IMessage reqMessage) where R : IMessage, new();
    }
}
