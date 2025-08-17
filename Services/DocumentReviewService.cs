using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Aspose.Cells;
using Aspose.Pdf.Operators;
using Aspose.Slides;
using AutoMapper;
using Core;
using Core.QueryModels;
using Database;
using Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Services.DTO;

namespace Services
{
    public interface IDocumentReviewService
    {

        Task<DocumentReviewDTO> UpdateDocumentReview(DocumentReviewDTO payload);
        Task BulkUpdateDocumentApprovalAsync(int documentId, int historicalId, List<Guid> userIds, Guid currentUserId);
        Task<DocumentReviewDTO> UpdateDocumentApprovalAsync(DocumentReviewDTO payload);
        Task<DocumentReviewDTO> GetSingleByUserIdAndDocId(Guid userId, int docId);
        Task<List<DocumentReviewDTO>> GetIndividualApprovalList(Guid userId);
        Task<IEnumerable<DocumentReviewDTO>> GetAllDocumentsApprovalAsync();  
        Task<List<DocumentApprovalSummaryDTO>> GetApprovalSummary(int id);
        Task<List<DocumentApprovalSummaryDTO_V2>> GetApprovalSummary_V2(DocumentSummaryQueryModel queries);

        Task<DocumentReviewDTO> GetItemByUserIdAsync(Guid userId);
        Task<DocumentReviewDTO> DocumentReadFileAsync(DocumentReadFileDTO payload);
        Task BulkUpdateDocumentApprovalAsyncForReUpdate(int documentId, int historicalId, List<Guid> userIds, Guid currentUserId);
    }
    public class DocumentReviewService : IDocumentReviewService
    {
        private readonly IMapper mapper;
        private readonly QLTTrContext qLTTrContext;
        private readonly IDocumentService documentService;
        private readonly IDocumentReviewRepository documentReviewRepository;    
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IUserInRoleRepository userInRoleRepository;
        private readonly IDocumentHistoryRepository _documentHistoryRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentFileRepository _documentFileRepository;

        public DocumentReviewService(IDocumentReviewRepository documentReviewRepository, IMapper mapper, QLTTrContext qLTTrContext, IDocumentService documentService,
            IUserInRoleRepository userInRoleRepository, IUserRepository userRepository, IRoleRepository roleRepository,IDocumentHistoryRepository documentHistoryRepository, IDocumentRepository documentRepository,IDocumentFileRepository documentFileRepository)
        {
            this.mapper = mapper;
            this.qLTTrContext = qLTTrContext;
            this.documentService = documentService;
            this.documentReviewRepository = documentReviewRepository;   
            this.userRepository = userRepository;
            this.userInRoleRepository = userInRoleRepository;
            this.roleRepository = roleRepository;
            _documentHistoryRepository = documentHistoryRepository;
            _documentRepository = documentRepository;
            _documentFileRepository = documentFileRepository;
        }
        public async Task<DocumentReviewDTO> UpdateDocumentReview(DocumentReviewDTO payload)
        {
            try
            {
                if (payload.DocId == null)
                {
                    throw new Exception("DocId cannot be null.");
                }
                var approvers = await GetUserIdsByDocumentReviewAsync(payload.DocId.Value);
                if (approvers == null || !approvers.Contains((Guid)payload.UserId))
                {
                    throw new Exception("Bạn không có quyền phê duyệt tài liệu này.");
                }
                var existingReview = await documentReviewRepository
                    .FirstOrDefaultAsync(dr => dr.DocId == payload.DocId && dr.UserId == payload.UserId&& dr.SubmitCount == payload.SubmitCount);
                if (existingReview != null)
                {
                    existingReview.ReviewResult = payload.ReviewResult;
                    existingReview.Comment = payload.Comment;
                    existingReview.Modified = DateTime.Now;
                    existingReview.Viewed = true;
                    if (payload.Attachment != null)
                    {
                        foreach (var file in payload.Attachment)
                        {
                            if (file.Length > 0)
                            {
                                var filePathResult = await documentService.UploadFile(file);

                                var documentFile = new TblDocumentFile
                                {
                                    Id = 0,
                                    FileName = file.FileName,
                                    FilePath = filePathResult.Item1,
                                    FilePathToView = filePathResult.Item2,
                                    DocId = existingReview.DocId,
                                    UserId = payload.UserId,
                                    Modified = DateTime.Now,
                                    Created = DateTime.Now,
                                    Version = 1, //documentData.SubmitCount,
                                    Deleted = false,
                                    CreatedBy = payload.UserId,
                                    ModifiedBy = payload.ModifiedBy,
                                    IsFinal = false,
                                    FileType = 2
                                };

                                _documentFileRepository.Add(documentFile);
                                await _documentFileRepository.SaveChanges();
                            }
                        }
                    }
                    documentReviewRepository.Update(existingReview);
                }
                else
                {
                    throw new Exception("No record found to update.");
                }
                await documentReviewRepository.SaveChanges();
                var result = mapper.Map<TblDocumentReview, DocumentReviewDTO>(existingReview);
                //var isAllReviewed = await UpdateDocumentStatusAndCreateHistoryAsync(payload.DocId.Value, "phe-duyet", "Tờ trình được gửi sang Phê duyệt",
                //                                                                    "Đã kết thúc quá trình xin ý kiến", 3);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<bool> UpdateDocumentStatusAndCreateHistoryAsync(int docId, string newStatus, string note, string comment, int documentStatus)
        {
            var userIds = await GetUserIdsByDocumentReviewAsync(docId);

            if (!userIds.Any())
            {
                return false;
            }

            var reviews = await documentReviewRepository.FindAsync(dr => dr.DocId == docId && dr.UserId.HasValue);
            var reviewedUserIds = reviews
                .Where(dr => dr.ReviewResult != null)
                .Select(dr => dr.UserId.Value)
                .Distinct()
                .ToList();

            if (userIds.All(uid => reviewedUserIds.Contains(uid)))
            {
                var document = await _documentRepository.FirstOrDefaultAsync(d => d.Id == docId);
                if (document != null)
                {
                    document.StatusCode = newStatus;
                    document.Modified = DateTime.Now;
                    _documentRepository.Update(document);
                    await _documentRepository.SaveChanges();
                    using (var transaction = await qLTTrContext.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            var history = new TblDocumentHistory
                            {
                                DocumentTitle = document.Title,
                                DocumentId = docId,
                                Note = note,
                                Comment = comment,
                                DocumentStatus = documentStatus,
                                CreatedBy = document.CreatedBy,
                                Created = DateTime.Now
                            };
                            await _documentHistoryRepository.AddAsync(history);
                            await _documentHistoryRepository.SaveChanges();

                            var documentHistoryId = history.Id;

                            var documentReview = new TblDocumentReview
                            {
                                DocId = docId,
                                DocumentHistoryId = documentHistoryId,
                                UserId = document.CreatedBy,
                                Created = DateTime.Now
                            };
                            await documentReviewRepository.AddAsync(documentReview);
                            await documentReviewRepository.SaveChanges();
                            await transaction.CommitAsync();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();

                            throw new Exception("Đã xảy ra lỗi trong quá trình xử lý", ex);
                        }
                    }
                  
                }
            }
            return false;
        }

        public async Task<DocumentReviewDTO> GetItemByUserIdAsync(Guid userId)
        {
            try
            {
                var data = await documentReviewRepository.FirstOrDefaultAsync(dr => dr.UserId == userId);

                if (data == null)
                {
                    throw new Exception("Document information for this UserId was not found.");
                }
                return mapper.Map<TblDocumentReview, DocumentReviewDTO>(data);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving data: {ex.Message}");
            }
        }
        private async Task<List<Guid>> GetUserIdsByDocumentReviewAsync(int docId)
        {
            var documentReviews = await documentReviewRepository.FindAsync(dr => dr.DocId == docId);
            var currentDocument = await _documentRepository.FirstOrDefaultAsync(d => d.Id == docId);

            return documentReviews
                .Where(dr => dr.UserId.HasValue && dr.DocumentHistoryId == currentDocument.CurrentDocumentHistoricalId)
                .Select(dr => dr.UserId.Value)
                .Distinct()
                .ToList();
        }
        private int? GetLatestDocumentHistoryId(int docId)
        {
            var allHistories = _documentHistoryRepository.GetAll();

            var latestHistory = allHistories
                .Where(h => h.DocumentId == docId)
                .OrderByDescending(h => h.Created)
                .FirstOrDefault();

            return latestHistory?.Id;
        }
        public async Task<IEnumerable<DocumentReviewDTO>> GetAllDocumentsApprovalAsync()
        {
            try
            {
                var docApprovalData = documentReviewRepository.GetAll();

                
                return mapper.Map<IEnumerable<DocumentReviewDTO>>(docApprovalData);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DocumentReviewDTO> GetSingleByUserIdAndDocId(Guid userId, int docId)
        {
            try
            {
                //var data = qLTTrContext.TblDocumentApprovals.Where(d => d.UserId == userId).Where(d => d.DocId == docId);

                var data = await documentReviewRepository.FirstOrDefaultAsync(d => d.UserId == userId && d.DocId == docId && d.Viewed == false);
                if(data == null)
                {
                    throw new Exception("Không tìm thấy thông tin tờ trình");
                }

                return mapper.Map<TblDocumentReview, DocumentReviewDTO>(data);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DocumentReviewDTO> UpdateDocumentApprovalAsync(DocumentReviewDTO payload)
        {
            try
            {
                var docApprovalData = await documentReviewRepository.FirstOrDefaultAsync(x => x.DocId == payload.DocId && x.UserId == payload.UserId);
                if(docApprovalData != null)
                {
                    docApprovalData.Title = payload.Title;
                    docApprovalData.DocId = payload.DocId;
                    docApprovalData.ReviewResult = payload.ReviewResult;
                    docApprovalData.UserId = payload.UserId;
                    docApprovalData.Modified = payload.Modified;
                    docApprovalData.Deleted = payload.Deleted;
                    docApprovalData.ModifiedBy = payload.ModifiedBy;
                    docApprovalData.CreatedBy = payload.CreatedBy;
                    docApprovalData.Created = payload.Created;
                    docApprovalData.Comment = payload.Comment;

                    await documentReviewRepository.SaveChanges();
                }
                //else
                //{
                //    return await CreateDocumentReview(payload);
                //}
                return mapper.Map<TblDocumentReview, DocumentReviewDTO>(docApprovalData);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task BulkUpdateDocumentApprovalAsync(int documentId, int historicalId, List<Guid> userIds, Guid currentUserId)
        {
            try
            {
                
               
                var userRoles = await documentService.GetRolesByUserId(currentUserId);
                bool isHighLevelLeader = userRoles != null &&
                                 (userRoles.Contains(AppRoleNames.BI_THU) || userRoles.Contains(AppRoleNames.PHO_BI_THU));
                var existingDocumentApprovals = await documentReviewRepository
                    .GetMultiAsNoTracking(x => x.DocId == documentId);
                var documentApprovalDataToUpdate = existingDocumentApprovals
                    .Where(dr => dr.UserId.HasValue && !userIds.Contains(dr.UserId.Value))
                    .ToList();

                foreach (var docApprovalEntity in documentApprovalDataToUpdate)
                {
                    var trackedEntity = await documentReviewRepository.GetSingleByCondition(x => x.Id == docApprovalEntity.Id);
                    if (trackedEntity != null)
                    {
                        if (isHighLevelLeader && trackedEntity.IsAssigned == true)
                        {
                            trackedEntity.IsHighLevelLeader = true;
                        }
                        trackedEntity.Modified = DateTime.Now;
                        trackedEntity.ModifiedBy = docApprovalEntity.UserId;
                        trackedEntity.IsAssigned = false;
                        documentReviewRepository.Update(trackedEntity);
                    }
                }
                await documentReviewRepository.SaveChanges();
                var documentApprovalDataToAdd = userIds
                    .Select(userId => new TblDocumentReview
                    {
                        DocId = documentId,
                        ReviewResult = null,
                        UserId = userId,
                        Modified = DateTime.Now,
                        ModifiedBy = userId,
                        CreatedBy = currentUserId,
                        Created = DateTime.Now,
                        Viewed = false,
                        SubmitCount = null,
                        DocumentHistoryId = historicalId,
                        IsAssigned = true,
                        IsHighLevelLeader = isHighLevelLeader

                    })
                    .ToList();
                if (documentApprovalDataToAdd.Any())
                {
                    await documentReviewRepository.AddRangeAsync(documentApprovalDataToAdd);
                }
                await documentReviewRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task BulkUpdateDocumentApprovalAsyncForReUpdate(int documentId, int historicalId, List<Guid> userIds, Guid currentUserId)
        {
            try
            {
                var documentReviews = await documentReviewRepository.GetMulti(x => x.DocId == documentId && x.IsAssigned == true);

                if (documentReviews != null && documentReviews.Any())
                {
                    foreach (var review in documentReviews)
                    {
                        review.IsAssigned = false;
                    }
                    documentReviewRepository.UpdateRange(documentReviews);
                    await documentReviewRepository.SaveChanges();
                }
                var documentApprovalData = new List<TblDocumentReview>();
                foreach (var userId in userIds)
                {
                    var docApprovalEntity = new TblDocumentReview
                    {
                        DocId = documentId,
                        ReviewResult = null,
                        UserId = userId,
                        Modified = DateTime.Now,
                        ModifiedBy = userId,
                        CreatedBy = currentUserId,
                        Created = DateTime.Now,
                        Viewed = false,
                        SubmitCount = null,
                        DocumentHistoryId = historicalId,
                        IsAssigned = true
                    };
                    documentApprovalData.Add(docApprovalEntity);
                }
                await documentReviewRepository.AddRangeAsync(documentApprovalData);
                await documentReviewRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<DocumentApprovalSummaryDTO>> GetApprovalSummary(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DocumentApprovalSummaryDTO_V2>> GetApprovalSummary_V2(DocumentSummaryQueryModel queries)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DocumentReviewDTO>> GetIndividualApprovalList(Guid userId)
        {
            try
            {
                var docApprovalData = await documentReviewRepository.GetMulti(x => x.UserId == userId && x.Viewed == false);


                return mapper.Map<List<DocumentReviewDTO>>(docApprovalData);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private async Task<string> UploadFile(IFormFile file)
        //{
        //    if (file.Length > 0)
        //    {
        //        var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
        //        var fileExtension = Path.GetExtension(file.FileName);
        //        var currentTime = DateTime.Now.ToString("yyyyMMddHHmmssfffttt");
        //        var fileName = $"{originalFileName}_{currentTime}{fileExtension}";
        //        var uploadFolderPath = Path.Combine("wwwroot", @"Files\Comment_Attachments");
        //        var filePath = Path.Combine(uploadFolderPath, fileName);


        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(fileStream);
        //        }

        //        var fullFilePath = "/Files/Comment_Attachments/" + fileName;
        //        return fullFilePath;
        //    }
        //    return "";
        //}
        public async Task<Tuple<string, string>> UploadFile(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var fileExtension = Path.GetExtension(file.FileName);
                    var currentTime = DateTime.Now.ToString("yyyyMMddHHmmssfffttt");
                    var fileName = $"{originalFileName}_{currentTime}{fileExtension}";
                    var uploadFolderPath = Path.Combine("wwwroot", @"Files\Document_Attachments");
                    var filePath = Path.Combine(uploadFolderPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var fullFilePath = "/Files/Document_Attachments/" + fileName;
                    return new Tuple<string, string>(fullFilePath, fullFilePath);
                }

                return new Tuple<string, string>("", "");
            }
            catch (Exception ex)
            {
                return new Tuple<string, string>("", "");
            }
        }

        public async Task<DocumentReviewDTO> DocumentReadFileAsync(DocumentReadFileDTO payload)
        {
            var currentDocument = await _documentRepository.FirstOrDefaultAsync(d => d.Id == payload.DocId);
            if (currentDocument == null)
            {
                throw new Exception("Document not found.");
            }

            var currentReview = await documentReviewRepository
                .FirstOrDefaultAsync(
                dr => dr.DocId == payload.DocId 
                && dr.UserId == payload.UserId
                && dr.DocumentHistoryId == currentDocument.CurrentDocumentHistoricalId);
            var currentReviewDto = new DocumentReviewDTO();

            if (currentReview == null)
            {
                var newReview = new TblDocumentReview
                {
                    DocId = payload.DocId,
                    UserId = payload.UserId,
                    DocumentHistoryId = currentDocument.CurrentDocumentHistoricalId,
                    Created = DateTime.Now,
                    CreatedBy = payload.UserId,
                    Viewed = true
                };

                await documentReviewRepository.AddAsync(newReview);
                await documentReviewRepository.SaveChanges();

                currentReviewDto =  mapper.Map<TblDocumentReview, DocumentReviewDTO>(newReview);
            }
            else
            {
                currentReview.Viewed = true;
                currentReview.Modified = DateTime.Now;
                currentReview.ModifiedBy = payload.UserId;
                documentReviewRepository.Update(currentReview);
                await documentReviewRepository.SaveChanges();
                currentReviewDto = mapper.Map<TblDocumentReview, DocumentReviewDTO>(currentReview);
            }

            return currentReviewDto;
        }
    }
}
