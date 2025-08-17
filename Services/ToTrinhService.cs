using Core;
using Core.Domains;
using Database.Models;
using Microsoft.AspNetCore.Http;
using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IToTrinhService
    {
        Task<BaseResponseModel> UpdateAsync(ToTrinhUpdateRequestDTO payload);
        Task<BaseResponseModel> ReUpdateAsync(ToTrinhUpdateRequestDTO payload);
        Task<BaseResponseModel> GetTotalWorkFlowByUser(Guid userId);
    }

    public class ToTrinhService : IToTrinhService
    {
        private readonly IDocumentService _documentService;
        private readonly ICategoryStatusesService _categoryStatusesService;
        private readonly IDocumentHistoryService _documentHistoryService;
        private readonly IDocumentReviewService _documentApprovalService;

        private readonly IStatusesRepository _statusesRepository;
        private readonly IDocumentFileRepository _documentFileRepository;
        private readonly IDocumentRepository _documentRepository;

        private BaseResponseModel _baseResponseModel;

        public ToTrinhService(
            IDocumentService documentService, 
            ICategoryStatusesService categoryStatusesService,
            IDocumentHistoryService documentHistoryService,
            IDocumentReviewService documentApprovalService,
            IStatusesRepository statusesRepository,
            IDocumentFileRepository documentFileRepository, IDocumentRepository documentRepository)
        {
            _documentService = documentService;
            _categoryStatusesService = categoryStatusesService;
            _baseResponseModel = new BaseResponseModel();
            _documentHistoryService = documentHistoryService;
            _documentApprovalService = documentApprovalService;
            _statusesRepository = statusesRepository;
            _documentFileRepository = documentFileRepository;
            _documentRepository = documentRepository;
        }

        public async Task<BaseResponseModel> UpdateAsync(ToTrinhUpdateRequestDTO payload)
        {
            _baseResponseModel.IsSuccess = true;
            await ValidateStatusAsync(payload);
            var currentDocument = await _documentService.GetDocumentById(payload.DocumentId, payload.UserId.ToString());
            ValidateDocument(currentDocument);

            if (!_baseResponseModel.IsSuccess)
            {
                return _baseResponseModel;
            }

            if (payload.AttachmentFiles != null)
            {
                foreach (var file in payload.AttachmentFiles)
                {
                    if (file.Length > 0)
                    {
                        var filePathResult = await _documentService.UploadFile(file);

                        var documentFile = new TblDocumentFile
                        {
                            Id = 0,
                            FileName = file.FileName,
                            FilePath = filePathResult.Item1,
                            FilePathToView = filePathResult.Item2,
                            DocId = currentDocument.Id,
                            UserId = payload.UserId,
                            Created = DateTime.Now,
                            Version = 1, //documentData.SubmitCount,
                            Deleted = false,
                            CreatedBy = payload.UserId,
                            IsFinal = false,
                            FileType = 0
                        };

                        _documentFileRepository.Add(documentFile);
                        await _documentFileRepository.SaveChanges();
                    }
                }
            }

            var toStatus = await _statusesRepository.FirstOrDefaultAsync(x => x.StatusCode == payload.ToStatusCode);
            // add new document history
            var newDocHistory = await _documentHistoryService.Create(new DocumentHistoryDTO
            {
                DocumentId = payload.DocumentId,
                DocumentTitle = currentDocument.Title,
                DocumentStatus = toStatus.Id,
                CreatedBy = payload.UserId,
                Comment = payload.Comment
            });

            // update document status
            await _documentService.UpdateApprovalByHistiricalId(payload.DocumentId, newDocHistory.Id, toStatus.StatusCode);

            // assign users
            await _documentApprovalService.BulkUpdateDocumentApprovalAsync(payload.DocumentId, newDocHistory.Id, payload.Users, payload.UserId);

            return new BaseResponseModel
            {
                IsSuccess = true,
                Message = "Update successfully",
                Result = newDocHistory,
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<BaseResponseModel> ReUpdateAsync(ToTrinhUpdateRequestDTO payload)
        {
            var userDocument = await _documentRepository.FirstOrDefaultAsync(x => x.Id == payload.DocumentId && x.CreatedBy.ToString() == payload.Users[0].ToString());
            if (userDocument == null)
            {
                return new BaseResponseModel
                {
                    IsSuccess = false,
                    Message = "Cán bộ đã chọn sai người. Vui lòng chọn lại đúng người đã tạo tờ trình.",
                    Result = null,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            var updatedDocument = await _documentService.ReUpdateDocumentAsync(payload.DocumentId);

            if (updatedDocument == null)
            {
                return new BaseResponseModel
                {
                    IsSuccess = false,
                    Message = $"NOT FOUND {payload.DocumentId}",
                    Result = null,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
           
            var currentDocument = await _documentService.GetDocumentById(payload.DocumentId, payload.UserId.ToString());
            ValidateDocument(currentDocument);
            if (payload.AttachmentFiles != null)
            {
                foreach (var file in payload.AttachmentFiles)
                {
                    if (file.Length > 0)
                    {
                        var filePathResult = await _documentService.UploadFile(file);

                        var documentFile = new TblDocumentFile
                        {
                            Id = 0,
                            FileName = file.FileName,
                            FilePath = filePathResult.Item1,
                            FilePathToView = filePathResult.Item2,
                            DocId = currentDocument.Id,
                            UserId = payload.UserId,
                            Created = DateTime.Now,
                            Version = 1, //documentData.SubmitCount,
                            Deleted = false,
                            CreatedBy = payload.UserId,
                            IsFinal = false,
                            FileType = 0
                        };

                        _documentFileRepository.Add(documentFile);
                        await _documentFileRepository.SaveChanges();
                    }
                }
            }
            var toStatus = await _statusesRepository.FirstOrDefaultAsync(x => x.StatusCode == AppDocumentStatuses.XIN_Y_KIEN_LAI);
            // add new document history
            var newDocHistory = await _documentHistoryService.Create(new DocumentHistoryDTO
            {
                DocumentId = payload.DocumentId,
                DocumentTitle = updatedDocument.Title,
                DocumentStatus = toStatus.Id,
                CreatedBy = payload.UserId,
                Comment = payload.Comment,
                SubmitCount = (int)updatedDocument.SubmitCount
            });

            // update document status
           //  await _documentService.UpdateApprovalByHistiricalId(payload.DocumentId, newDocHistory.Id, AppDocumentStatuses.XIN_Y_KIEN_LAI);

            // assign users
            await _documentApprovalService.BulkUpdateDocumentApprovalAsyncForReUpdate(payload.DocumentId, newDocHistory.Id, payload.Users, payload.UserId);

            return new BaseResponseModel
            {
                IsSuccess = true,
                Message = "Update successfully",
                Result = newDocHistory,
                StatusCode = StatusCodes.Status200OK
            };
        }

        private async Task ValidateStatusAsync(ToTrinhUpdateRequestDTO payload)
        {
            var isValidStatus = await _categoryStatusesService.ValidateStatusAsync(payload.FromStatusCode, payload.ToStatusCode);

            if (!isValidStatus)
            {
                _baseResponseModel.StatusCode = StatusCodes.Status400BadRequest;
                _baseResponseModel.Message = "Invalid status";
                _baseResponseModel.IsSuccess = false;
            }
        }

        private void ValidateDocument(DocumentDTO currentDocument)
        {
            if (currentDocument == null)
            {
                _baseResponseModel.StatusCode = StatusCodes.Status404NotFound;
                _baseResponseModel.Message = "Document not found";
                _baseResponseModel.IsSuccess = false;
            }
        }

        public async Task<BaseResponseModel> GetTotalWorkFlowByUser(Guid userId)
        {
            var data = await _statusesRepository.GetStatusByUser(userId);

            if (data == null)
            {
                return new BaseResponseModel
                {
                    IsSuccess = false,
                    Message = "Data not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            return new BaseResponseModel
            {
                IsSuccess = true,
                Result = data,
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}
