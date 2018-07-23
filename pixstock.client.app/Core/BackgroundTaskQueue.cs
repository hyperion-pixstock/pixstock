using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using pixstock.apl.app.core.Infra;

namespace pixstock.apl.app.core
{
  public class BackgroundTaskQueue : IBackgroundTaskQueue
  {
    private readonly ILogger mLogger;

    private ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new ConcurrentQueue<Func<CancellationToken, Task>>();

    private SemaphoreSlim _signal = new SemaphoreSlim(0);

    public BackgroundTaskQueue()
    {
      mLogger = LogManager.GetCurrentClassLogger();
    }

    public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
    {
      if (workItem == null)
      {
        throw new ArgumentNullException(nameof(workItem));
      }

      _workItems.Enqueue(workItem);
      _signal.Release();
    }

    public async Task<Func<CancellationToken, Task>> DequeueAsync(
        CancellationToken cancellationToken)
    {
      mLogger.Debug("[DequeueAsync] IN");
      await _signal.WaitAsync(cancellationToken);
      _workItems.TryDequeue(out var workItem);

      return workItem;
    }
  }
}
