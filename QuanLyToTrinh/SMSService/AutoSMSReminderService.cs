using Core;
using Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Models;
using QuanLyToTrinh.AutoApprovingDocument;

namespace QuanLyToTrinh.SMSService
{
    public class AutoSMSReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private DateTime _lastProcessedTime;
        private readonly HashSet<int> _processedDocs;
        public static bool IsSMSSuccessful { get; set; }
        private readonly ILogger<AutoSMSReminderService> _logger;
        public AutoSMSReminderService(IServiceScopeFactory serviceScopeFactory, ILogger<AutoSMSReminderService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _lastProcessedTime = DateTime.MinValue;
            _processedDocs = new HashSet<int>();
            IsSMSSuccessful = false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var documentRepository = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
                        var smsService = scope.ServiceProvider.GetRequiredService<ISMSService>();
                        var result = await AutoSMSReminder(documentRepository, smsService);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the autoSMSRemind.");
                   
                }
                await Task.Delay(TimeSpan.FromMinutes(1.2), stoppingToken);
            }
        }


        private async Task<List<bool>> AutoSMSReminder(IDocumentRepository documentRepository, ISMSService smsService)
        {
            var result = new List<bool>();
            var currentTime = DateTime.Now;
            Console.WriteLine($"Current time: {currentTime}");

            if (_lastProcessedTime.AddMinutes(1.2) < currentTime)
            {
                _lastProcessedTime = currentTime.AddMinutes(-1.2);

                _logger.LogInformation("Updated _lastProcessedTime: {LastProcessedTime}", _lastProcessedTime);
            }
            var docList = (await documentRepository.GetMulti(x =>
                x.StatusCode == AppDocumentStatuses.XIN_Y_KIEN &&
                ((DateTime)x.RemindDatetime) > _lastProcessedTime &&
                ((DateTime)x.RemindDatetime) <= currentTime &&
                !_processedDocs.Contains(x.Id)))
                .ToList();
            Console.WriteLine($"Documents found: {docList.Count}");
            var sendTasks = docList.Select(doc =>
            {
                return Task.Run(async () =>
                {
                    _logger.LogInformation("Running task for document {DocumentId}...", doc.Id);
                    var sendResults = await smsService.SendSMSV2(doc.Id, 2);
                    _logger.LogInformation("SMS sent for document {DocumentId}, results: {SendResults}", doc.Id, string.Join(", ", sendResults));

                    return sendResults;
                });
            }).ToList();
            var sendResults = await Task.WhenAll(sendTasks);
            foreach (var sendResultList in sendResults)
            {
                foreach (var sendResult in sendResultList)
                {
                    result.Add(sendResult);
                    _logger.LogInformation("Task result: {SendResult}", sendResult);
                }
            }
            foreach (var doc in docList)
            {
                _processedDocs.Add(doc.Id);
                _logger.LogInformation("Processed document: {DocumentId}", doc.Id);
            }
            if (docList.Any())
            {
                _lastProcessedTime = docList.Max(x => (DateTime)x.RemindDatetime);
                Console.WriteLine($"Updated _lastProcessedTime to max document remind time: {_lastProcessedTime}");
            }
            else
            {
                _lastProcessedTime = currentTime;
                Console.WriteLine($"No documents found, setting _lastProcessedTime to current time: {_lastProcessedTime}");
            }

            return result;
        }
       
    }
}