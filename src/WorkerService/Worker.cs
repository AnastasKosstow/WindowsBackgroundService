using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WorkerService.Configuration;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        public Worker(
            ILogger<Worker> logger,
            IHttpClientFactory httpClientFactory,
            WorkerServiceConfiguration workerServiceConfiguration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _workerServiceConfiguration = workerServiceConfiguration;
        }


        /// <summary>
        ///     Serilog
        /// </summary>
        private readonly ILogger<Worker> _logger;

        /// <summary>
        ///     Hosted service configuration.
        /// </summary>
        private readonly WorkerServiceConfiguration _workerServiceConfiguration;

        /// <summary>
        ///     System.Net.Http.HttpClient instance
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;


        private IList<string> Urls = new List<string>
        {
            "https://www.google.com",
            "https://www.somefakeurl.com"
        };


        /// <inheritdoc cref="BackgroundService"/>
        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        /// <seealso cref="BackgroundService"/>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting hosted service...");
            await base.StartAsync(cancellationToken);
            _logger.LogInformation("Hosted service started.");
        }


        /// <inheritdoc cref="BackgroundService"/>
        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        /// <seealso cref="BackgroundService"/>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping hosted service...");
            await base.StopAsync(cancellationToken);
            _logger.LogInformation("Hosted service stopped.");
        }


        /// <inheritdoc cref="BackgroundService"/>
        /// <summary>
        /// This method is called when the <see cref="BackgroundService"/> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="BackgroundService.StopAsync(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        /// <seealso cref="BackgroundService"/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    var tasks = Urls
                        .Select(url => PollUrl(url))
                        .ToList();
                    await Task.WhenAll(tasks);
                }
                finally
                {
                    await Task.Delay(_workerServiceConfiguration.IntervalInMilliseconds, stoppingToken);
                }
        }

        private async Task PollUrl(string url)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                    _logger.LogInformation($"{url} is online.");

                else
                    _logger.LogWarning($"{url} is offline.");
            }
            catch
            {
                _logger.LogWarning($"{url} is offline.");
            }
        }
    }
}
