using Core;
using Database.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuanLyToTrinh.SMSService;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuanLyToTrinh.AutoApprovingDocument
{
    public class AutoApprovingService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AutoApprovingService> _logger;
        private Timer _timer;
        private DateTime _lastProcessedTime;
        private readonly Dictionary<int, DateTime> _processedDocs = new();
        private readonly TimeSpan _rentTime = TimeSpan.FromMinutes(10); 

        public AutoApprovingService(IServiceScopeFactory serviceScopeFactory, ILogger<AutoApprovingService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _lastProcessedTime = DateTime.Now.AddMinutes(-2);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("AutoApprovingService is starting...");
            _timer = new Timer(ProcessDocuments, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); 
            return Task.CompletedTask;
        }

        private async void ProcessDocuments(object state)
        {
            try
            {
                CleanupProcessedDocs(); // Xóa các tài liệu đã hết hạn khỏi danh sách xử lý

                using var scope = _serviceScopeFactory.CreateScope();
                var documentRepository = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
                var documentReviewRepository = scope.ServiceProvider.GetRequiredService<IDocumentReviewRepository>();
                var documentHistoryRepository = scope.ServiceProvider.GetRequiredService<IDocumentHistoryRepository>();

                bool hasProcessed = await AutoApproveDocuments(documentRepository, documentReviewRepository, documentHistoryRepository);
                if (hasProcessed)
                {
                    _lastProcessedTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the auto-approving process.");
            }
        }

        private async Task<bool> AutoApproveDocuments(IDocumentRepository documentRepository, IDocumentReviewRepository documentReviewRepository, IDocumentHistoryRepository documentHistoryRepository)
        {
            var currentProcessingTime = DateTime.Now;
            _logger.LogInformation($"Auto-approving documents from {_lastProcessedTime} to {currentProcessingTime}");

            var documentsToApprove = await documentRepository.GetMulti(x =>
                x.DateEndApproval <= currentProcessingTime &&
                x.DateEndApproval > _lastProcessedTime &&
                x.StatusCode == AppDocumentStatuses.XIN_Y_KIEN);

            if (!documentsToApprove.Any())
            {
                _logger.LogInformation("No documents found for auto-approval.");
                return false;
            }

            foreach (var document in documentsToApprove)
            {
                // Kiểm tra xem tài liệu có nằm trong danh sách đã xử lý gần đây không
                if (_processedDocs.TryGetValue(document.Id, out var lastProcessedTime) && lastProcessedTime.Add(_rentTime) > DateTime.Now)
                {
                    _logger.LogInformation($"Skipping document {document.Id}, recently processed.");
                    continue;
                }

                // Đánh dấu tài liệu vào danh sách đã xử lý
                _processedDocs[document.Id] = DateTime.Now;

                var pendingReviews = await documentReviewRepository.GetMulti(r =>
                    r.DocId == document.Id && r.SubmitCount.HasValue && r.SubmitCount > 0);

                foreach (var review in pendingReviews)
                {
                    if (!review.ReviewResult.HasValue)
                    {
                        review.ReviewResult = 1;
                        review.Comment = "Hệ thống tự động gán Đồng ý";
                    }
                    review.Modified = DateTime.Now;
                    review.Viewed = false;
                    review.IsAssigned = false;
                }

                documentReviewRepository.UpdateRange(pendingReviews);
                await documentReviewRepository.SaveChanges();

                var allReviews = await documentReviewRepository.FindAsync(dr => dr.DocId == document.Id);
                var approvers = await GetUserIdsByDocumentReviewAsync(document.Id);
                var reviewedUserIds = allReviews
                    .Where(dr => dr.ReviewResult.HasValue)
                    .Select(dr => dr.UserId.Value)
                    .Distinct()
                    .ToList();

                if (approvers.All(uid => reviewedUserIds.Contains(uid)))
                {
                    var history = new TblDocumentHistory
                    {
                        DocumentTitle = document.Title,
                        DocumentId = document.Id,
                        Note = "Tờ trình được gửi sang Phê duyệt",
                        Comment = "Đã kết thúc quá trình xin ý kiến",
                        DocumentStatus = 3,
                        CreatedBy = document.CreatedBy,
                        Created = DateTime.Now
                    };
                    await documentHistoryRepository.AddAsync(history);
                    await documentHistoryRepository.SaveChanges();

                    document.StatusCode = AppDocumentStatuses.PHE_DUYET;
                    document.Modified = DateTime.Now;
                    document.CurrentDocumentHistoricalId = history.Id;
                    document.PreviousStatusCode = AppDocumentStatuses.XIN_Y_KIEN;

                    documentRepository.Update(document);
                    await documentRepository.SaveChanges();

                    var documentReview = new TblDocumentReview
                    {
                        DocId = document.Id,
                        DocumentHistoryId = history.Id,
                        UserId = document.CreatedBy,
                        Created = DateTime.Now,
                        Viewed = true,
                        IsAssigned = true,
                        CreatedBy = document.CreatedBy
                    };
                    await documentReviewRepository.AddAsync(documentReview);
                    await documentReviewRepository.SaveChanges();

                    _logger.LogInformation($"Document {document.Id} status updated to 'phe-duyet'");
                    _lastProcessedTime = currentProcessingTime;
                }
            }
            return true;
        }

        private async Task<List<Guid>> GetUserIdsByDocumentReviewAsync(int docId)
        {
            return new List<Guid>();
        }

        private void CleanupProcessedDocs()
        {
            var now = DateTime.Now;
            var expiredDocs = _processedDocs.Where(kv => kv.Value.Add(_rentTime) <= now).Select(kv => kv.Key).ToList();

            foreach (var docId in expiredDocs)
            {
                _processedDocs.Remove(docId);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("AutoApprovingService is stopping...");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
