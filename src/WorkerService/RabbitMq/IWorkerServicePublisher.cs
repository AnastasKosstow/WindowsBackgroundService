
namespace WorkerService.RabbitMq
{
    public interface IWorkerServicePublisher
    {
        void Publish(object data);
    }
}
