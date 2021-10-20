using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private IList<string> Urls = new List<string> 
        { 
            "https://www.google.com", 
            "https://www.somefakeurl.com/" 
        };

        public Worker(
            ILogger<Worker> logger, 
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
                await PollUrls();
        }


        private async Task PollUrls()
        {
            var tasks = Urls
                .Select(url => PollUrl(url))
                .ToList();
            await Task.WhenAll(tasks);
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
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"{url} is offline.");
            }
        }
    }
}
