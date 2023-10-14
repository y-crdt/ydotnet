using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDotNet.Server.Clustering
{
    public interface IPubSub
    {
        IDisposable Subscribe(Func<byte[], Task> handler);

        Task PublishAsync(byte[] payload,
            CancellationToken ct);
    }
}
