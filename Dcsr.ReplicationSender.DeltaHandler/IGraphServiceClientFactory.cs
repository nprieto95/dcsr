using Microsoft.Graph;

namespace Dcsr.ReplicationSender.Functions
{
    public interface IGraphServiceClientFactory
    {
        GraphServiceClient CreateGraphClient();
    }
}