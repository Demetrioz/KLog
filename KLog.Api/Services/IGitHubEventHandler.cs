using System.Threading.Tasks;

namespace KLog.Api.Services
{
    public interface IGitHubEventHandler
    {
        Task HandleEvent(int appId, object eventObject, string eventType = null);
    }
}
