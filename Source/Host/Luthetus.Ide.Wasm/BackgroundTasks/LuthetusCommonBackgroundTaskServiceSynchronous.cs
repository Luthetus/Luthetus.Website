//using System.Collections.Concurrent;
//using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;

//namespace Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;

//public class LuthetusCommonBackgroundTaskServiceSynchronous : ILuthetusCommonBackgroundTaskService
//{
//    private readonly object _taskLock = new();

//    private Task _activeTask = Task.CompletedTask;

//    public event Action? ExecutingBackgroundTaskChanged;

//    public IBackgroundTask? ExecutingBackgroundTask { get; private set; }

//    public void QueueBackgroundWorkItem(IBackgroundTask backgroundTask)
//    {
//        lock (_taskLock)
//        {
//            _activeTask = _activeTask.ContinueWith(_ =>
//                await backgroundTask.InvokeWorkItem(CancellationToken.None).);
//        }
//    }

//    public async Task<IBackgroundTask?> DequeueAsync(
//        CancellationToken cancellationToken)
//    {
//        IBackgroundTask? backgroundTask;

//        try
//        {
//            await _workItemsQueueSemaphoreSlim.WaitAsync(cancellationToken);

//            _backgroundTasks.TryDequeue(out backgroundTask);
//        }
//        finally
//        {
//            _workItemsQueueSemaphoreSlim.Release();
//        }

//        return backgroundTask;
//    }

//    public void SetExecutingBackgroundTask(IBackgroundTask? backgroundTask)
//    {
//        ExecutingBackgroundTask = backgroundTask;
//        ExecutingBackgroundTaskChanged?.Invoke();
//    }
//}
