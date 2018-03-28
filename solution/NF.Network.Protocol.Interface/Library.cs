using System.Threading.Tasks;
using NF.Results;

namespace NF.Network.Protocol.Interface
{
    public interface IMessageSender
    {
    }

    public interface INetworkSender<TBase>
    {
        Task<Result<TResponse, int>> Send<TResponse>(TBase msg) where TResponse : TBase, new();
    }
}
