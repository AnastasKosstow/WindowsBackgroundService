using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using WorkerService.RabbitMq;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly IWorkerServicePublisher _workerServicePublisher;

        public Worker(IWorkerServicePublisher workerServicePublisher)
            => _workerServicePublisher = workerServicePublisher;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
            }
        }
    }
}
