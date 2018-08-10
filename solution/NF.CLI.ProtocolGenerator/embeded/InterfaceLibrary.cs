using System.Threading.Tasks;
using NF.Results;
using Google.Protobuf;

namespace {{ interface_namespace }}
{
    public interface IMessageSender
    {
    }

    public interface INetworkSender<TBase>
    {
        Task<Result<TResponse, int>> Send<TResponse>(TBase msg) where TResponse : TBase, new();
    }
}
