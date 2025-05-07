using System.Threading;
using System.Threading.Tasks;

namespace Tsa.Submissions.Auth.WebApi.Services;

public interface IPingableService
{
    string ServiceName { get; }

    Task<bool> PingAsync(CancellationToken cancellationToken = default);
}
