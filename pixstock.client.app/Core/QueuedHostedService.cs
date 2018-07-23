using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NLog;
using pixstock.apl.app.core.Infra;

namespace pixstock.apl.app.core
{

    public class QueuedHostedService : IHostedService
    {
        private readonly ILogger mLogger;

        private CancellationTokenSource _shutdown = new CancellationTokenSource();

        readonly IBackgroundTaskQueue mBackgroundTaskQueue;

        private Task _backgroundTask;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="backgroundTaskQueue"></param>
        public QueuedHostedService(IBackgroundTaskQueue backgroundTaskQueue)
        {
            this.mBackgroundTaskQueue = backgroundTaskQueue;

      this.mLogger = LogManager.GetCurrentClassLogger();
    }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.mLogger.Debug("[QueuedHostedService][StartAsync] - CALL");
            _backgroundTask = Task.Run(BackgroundProceessing);
            this.mLogger.Debug("[QueuedHostedService][StartAsync] - RUN Task");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("Queued Hosted Service is stopping.");

            _shutdown.Cancel();

            return Task.WhenAny(_backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task BackgroundProceessing()
        {
            Console.WriteLine("[QueuedHostedService][BackgroundProceessing] - IN " + mBackgroundTaskQueue);

            while (!_shutdown.IsCancellationRequested)
            {
                var workItem = await mBackgroundTaskQueue.DequeueAsync(_shutdown.Token);
                try
                {
                    await workItem(_shutdown.Token);
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex,
                    //    $"Error occurred executing {nameof(workItem)}.");
                }
            }
        }
    }
}
