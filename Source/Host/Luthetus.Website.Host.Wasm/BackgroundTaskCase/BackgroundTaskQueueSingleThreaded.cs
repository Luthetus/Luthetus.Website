using System.Collections.Concurrent;
using System.Threading;

namespace Luthetus.Common.RazorLib.BackgroundTaskCase;

public class BackgroundTaskQueueSingleThreaded : IBackgroundTaskQueue
{
    public void QueueBackgroundWorkItem(
        IBackgroundTask backgroundTask)
    {
        backgroundTask
            .InvokeWorkItem(CancellationToken.None)
            .Wait();
    }

    public Task<IBackgroundTask?> DequeueAsync(
        CancellationToken cancellationToken)
    {
        return Task.FromResult(default(IBackgroundTask?));
    }
}