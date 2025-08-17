using AutoMapper;
using Core;
using Core.QueryModels;
using Database.Models;
using LinqKit;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories;
using Services.DTO;
using System.Linq.Expressions;
using System.Web.Http.Results;
using UploadFileServer.Controller;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using System.Data;
using Aspose.Pdf.Operators;
using System.IO.Compression;
using System.Security.Principal;
using System.Security.AccessControl;
using System.IO;
using System.Reflection.Metadata;
using SkiaSharp;
using Azure.Core;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Office2010.Word;


namespace Services
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentDTO>> GetDocumentsList(GetDocumentListQueryModel query);
        Task<IEnumerable<DocumentDTO>> GetAll();
        Task<DocumentDTO> GetDocumentById(int? id, string? assigneeId);
        Task<IEnumerable<DocumentFileDTO>> GetDocumentAttachments(int documentId);
        Task<IEnumerable<DocumentApprovalGroupedDTO>> GetDocumentApprovals(int documentId);
        Task<DocumentDTO> CreateAndSend(DocumentDTO payload);
        Task<DocumentDTO> CreateDraft(DocumentDTO payload);

        Task<DocumentDTO> UpdateDocument(DocumentDTO payload, IFormFile[] files);

        Task<DocumentDTO?> ReUpdateDocumentAsync(int documentId);

        //Task<int> UpdateDocumentStatus(int docId, int status, int handler);
        Task<int> RetrieveDocument(DocumentRetrievalRequest payload);
        Task<int> ReturnDocument(DocumentRetrievalRequest payload);
        Task<int> UpdatePriorityDocument(int docId, int priorityNumber);

        Task<int> DeleteDocument(int docId);
        Task<object> SignedFile(int docId, string userId);

        Task<object> ResultSignedFiles(int docId, string userId, int submitCount);
        Task<ChartRequests> GetDocumentsByCharMonth();
        Task<List<ChartDataByField>> GetDocumentsByChartField();
        Task<DocumentGroup> CountDocumentByStatus();
        Task<pieData> pieData();
        Task<TblDocumentFile> GetFinalPdf(int docid);
        Task<List<int>> UpdateApprovalStatus_V2();
        Task UpdateApprovalByHistiricalId(int documentId, int currentDocumentHistoryId, string statusCode);
        Task<int> PublishDocument(int docId, IFormFile[] files);
        Task<DocumentFileDTO> GDSignedFile(DocumentFileDTO document);
        Task<DocumentFileDTO> GDSignedFileForce(DocumentFileDTO document);
        Task<bool> SaveFile(IFormFile file, DocumentDTO payload, int fileType = 0);
        Task<byte[]> PrintResult(int docId, string userId, string comment, string currentUrl);
        Task<Tuple<string, string>> UploadFile(IFormFile file);
        Task<List<string>> GetRolesByUserId(Guid userId);
        Task<bool> ProcessApproving();
    }

    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IConfiguration _configuration;
        //private readonly IReviewUserDetailRepository _reviewUserDetailsRepository;
        private readonly IDocumentFileRepository _documentFileRepository;
        private readonly IDocumentHistoryRepository _documentHistoryRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserInRoleRepository _userInRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IDocumentReviewRepository _documentReviewRepository;
        private readonly INotificationService _notificationService;
        private readonly IDocumentHistoryService _historyService;
        private readonly IDocumentTypesRepository _documentTypesRepository;
        private readonly IStatusesRepository _statusRepository;
        private readonly ITblDepartmentRepository _departmentRepository;
        private readonly ICfgWorkFlowRepository _workflowRepository;
        private readonly ICfgWorkFlowUser _workflowUserRepository;
        private readonly QLTTrContext _context;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(
            IDocumentRepository documentRepository,
            IMapper mapper, QLTTrContext context,
            IDocumentFileRepository documentFileRepository,
            IDocumentHistoryRepository documentHistoryRepository,
            IDocumentReviewRepository documentReviewRepository,
            IUserRepository userRepository,
            IUserInRoleRepository userInRoleRepository,
            IRoleRepository roleRepository,
            IFieldRepository fieldRepository,
            ICommentRepository commentRepository,
            INotificationService notificationService,
            IDocumentHistoryService historyService,
            IDocumentTypesRepository documentTypesRepository,
            IStatusesRepository statusRepository,
            IConfiguration configuration,
            ITblDepartmentRepository departmentRepository,
            ICfgWorkFlowRepository workflowRepository,
            ICfgWorkFlowUser workflowUserRepository,
            //IReviewUserDetailRepository reviewUserDetailsRepository,
            ILogger<DocumentService> logger)
        {
            this._documentRepository = documentRepository;
            this._documentReviewRepository = documentReviewRepository;
            this._documentFileRepository = documentFileRepository;
            this._documentHistoryRepository = documentHistoryRepository;
            this._mapper = mapper;
            this._userRepository = userRepository;
            this._fieldRepository = fieldRepository;
            this._commentRepository = commentRepository;
            this._notificationService = notificationService;
            this._historyService = historyService;
            this._context = context;
            this._logger = logger;
            this._userInRoleRepository = userInRoleRepository;

            this._roleRepository = roleRepository;
            this._statusRepository = statusRepository;
            this._documentTypesRepository = documentTypesRepository;
            this._configuration = configuration;
            this._departmentRepository = departmentRepository;
            this._workflowRepository = workflowRepository;
            this._workflowUserRepository = workflowUserRepository;
        }
        public async Task<List<string>> GetRolesByUserId(Guid userId)
        {
            var roleIds = (await _userInRoleRepository.GetMulti(x => x.UserId == userId))
                            .Select(x => x.RoleId)
                            .ToList();
            var roles = (await _roleRepository.GetMulti(x => roleIds.Contains(x.RoleId)))
                            .Select(x => x.RoleName)
                            .ToList();
            return roles;
        }
        public async Task<IEnumerable<DocumentDTO>> GetDocumentsList(GetDocumentListQueryModel query)
        {
            var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.UserId.ToString() == query.CurrentUserId);
            if (currentUser == null) return new List<DocumentDTO>();
            var userRoles = await GetRolesByUserId(currentUser.UserId);
            bool isHighLevelLeader = userRoles.Contains(AppRoleNames.BI_THU) ||
                     userRoles.Contains(AppRoleNames.PHO_BI_THU);
            bool isTopManager = userRoles.Contains(AppRoleNames.CHANH_VAN_PHONG) ||
                     userRoles.Contains(AppRoleNames.PHO_CHANH_VAN_PHONG) || userRoles.Contains(AppRoleNames.ADMIN);
            bool isManager = userRoles.Contains(AppRoleNames.TRUONG_PHONG) || userRoles.Contains(AppRoleNames.PHO_TRUONG_PHONG);
            bool isChuyenVien = userRoles.Contains(AppRoleNames.CHUYEN_VIEN);
            bool isPendingSign = userRoles.Contains(AppRoleNames.BI_THU) ||
                     userRoles.Contains(AppRoleNames.PHO_BI_THU)|| userRoles.Contains(AppRoleNames.CHANH_VAN_PHONG )|| userRoles.Contains(AppRoleNames.PHO_CHANH_VAN_PHONG);
            var department = await _departmentRepository.FirstOrDefaultAsync(d => d.Id == currentUser.DepartmentId);
            bool isTongHopDepartment = department?.DepartmentName == "Tổng Hợp";
            //if (isTongHopDepartment) isTopManager = true;

            Expression<Func<TblDocument, bool>> filter = data => true;
            var allowedStatuses = new List<string>{
                   AppDocumentStatuses.XIN_Y_KIEN,
                   AppDocumentStatuses.BAN_HANH,
                   AppDocumentStatuses.KHONG_BAN_HANH
            };
            var assignedDocumentIds = (await _documentReviewRepository.GetMulti(x => x.UserId == currentUser.UserId && (x.IsAssigned == true || x.SubmitCount.HasValue && x.SubmitCount > 0 || x.IsHighLevelLeader == true))).Select(x => x.DocId).ToList();
            var departmentUserIds = (await _userRepository.GetMulti(u => u.DepartmentId != null && u.DepartmentId == currentUser.DepartmentId))
                        .Select(u => u.UserId).ToList();
            if (!isTopManager)
            {
                if (isTongHopDepartment)
                {
                    filter = filter.And(x => (departmentUserIds.Contains(x.CreatedBy) && x.StatusCode != AppDocumentStatuses.DU_THAO &&
                                     x.StatusCode != AppDocumentStatuses.XIN_Y_KIEN_LAI) || assignedDocumentIds.Contains(x.Id) || x.CreatedBy == currentUser.UserId);
                }
                else if (isManager)
                {

                    filter = filter.And(x => departmentUserIds.Contains(x.CreatedBy) || assignedDocumentIds.Contains(x.Id));

                }
                else if (isChuyenVien)
                {
                    filter = filter.And(x => x.CreatedBy == currentUser.UserId || (assignedDocumentIds.Contains(x.Id)));
                }
                else
                {
                    filter = isHighLevelLeader ? filter.And(x => assignedDocumentIds.Contains(x.Id)) :
                        filter.And(x => assignedDocumentIds.Contains(x.Id));
                }
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                filter = filter.And(x => x.StatusCode == query.Status);
            }
            else
            {
                filter = filter.And(x => x.StatusCode != AppDocumentStatuses.DU_THAO && x.StatusCode != AppDocumentStatuses.XIN_Y_KIEN_LAI || x.CreatedBy == currentUser.UserId);
            }
            var documents = await _documentRepository.Query(filter).ToListAsync();
            var documentIds = documents.Select(d => d.Id).ToList();
            var documentReviews = await _documentReviewRepository.GetMulti(d => documentIds.Contains(d.DocId ?? 0));
            var reviewUsers = await _userRepository.GetMulti(u => documentReviews.Select(r => r.UserId).Contains(u.UserId));
            var documentReviewDtos = documentReviews
                .Select(s => new DocumentReviewDTO
                {
                    Id = s.Id,
                    DocId = s.DocId,
                    UserId = s.UserId,
                    ReviewResult = s.ReviewResult,
                    UserName = reviewUsers.FirstOrDefault(a => a.UserId == s.UserId)?.UserFullName,
                    SubmitCount = s.SubmitCount,
                    DocumentHistoryId = s.DocumentHistoryId,
                    IsAssigned = s.IsAssigned,
                    CreatedBy = s.CreatedBy,
                    Created = s.Created
                }).ToList();
            var createdByIds = documents.Select(d => d.CreatedBy).Distinct().ToList();
            var authors = await _userRepository.GetMulti(x => createdByIds.Contains(x.UserId));
            var authorDict = authors.ToDictionary(x => x.UserId, x => x.UserFullName);
            var resultList = _mapper.Map<List<DocumentDTO>>(documents);
            var documentHisIds =  _documentHistoryRepository.GetAll();
            foreach (var document in resultList)
            {
                document.AuthorName = authorDict.TryGetValue((Guid)document.CreatedBy, out var authorName) ? authorName : "";
                document.FieldName = (await _fieldRepository.FirstOrDefaultAsync(x => x.Id == document.FieldId))?.Title;
                document.TypeName = (await _documentTypesRepository.FirstOrDefaultAsync(x => x.Id == document.TypeId))?.Name;
                var documentReview = documentReviewDtos.Where(d => d.DocId == document.Id && d.UserId == currentUser.UserId && d.IsAssigned == true).FirstOrDefault();
                document.AssigneeId = (documentReview != null) ? documentReview.UserId : null;
                document.AssigneeName = documentReview?.UserName;
                if (query.Status == AppDocumentStatuses.XIN_Y_KIEN && documentReview != null)
                {
                    document.ReviewStatus = documentReview.ReviewResult != null ? "Đã xử lý"
                            : documentReview.SubmitCount != null ? "Chưa xử lý"
                            : document.ReviewStatus;
                }
                var lastDocumentReview = documentReviewDtos
                    .Where(x => x.DocId == document.Id)
                    .OrderBy(x => x.Created)
                    .LastOrDefault();
                if (lastDocumentReview != null && lastDocumentReview.CreatedBy.ToString() == query.CurrentUserId && isPendingSign 
                    && document.StatusCode == AppDocumentStatuses.KY_SO)
                {
                    document.IsTransferredSign = true;
                }
                else
                {
                    document.IsTransferredSign = false;
                }
                var documentHistory = documentHisIds.FirstOrDefault(x => x.Id == document.CurrentDocumentHistoricalId);
                document.IsReturned = documentHistory?.Comment == "Tôi xác nhận trả về";
            }
            return resultList;
        }
        public async Task<DocumentDTO> GetDocumentById(int? id, string? assigneeId)
        {
            var data = _documentRepository.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                throw new Exception($"Document of ID [{id}] not found");
            }

            var result = _mapper.Map<DocumentDTO>(data);

            var userData = await _userRepository.FirstOrDefaultAsync(x => x.UserId == data.CreatedBy);
            if (userData != null) result.AuthorName = userData.UserFullName;
            var fieldData = await _fieldRepository.FirstOrDefaultAsync(x => x.Id == data.FieldId);
            if (fieldData != null) result.FieldName = fieldData.Title;

            if (!string.IsNullOrEmpty(assigneeId))
            {
                var user = await _userRepository.FirstOrDefaultAsync(u => u.UserId.ToString() == assigneeId);
                result.AssigneeId = user?.UserId;
                result.AssigneeName = user?.UserFullName;
            }
            var userInDocuMentReview = await _documentReviewRepository.GetMulti(x => x.DocId == id &&
              ((data.PreviousStatusCode == null && x.SubmitCount.HasValue) || x.SubmitCount == data.SubmitCount));
            var (infoUser, roles) = await _userRepository.GetAllUserWithRolesAsync();

            if (infoUser == null || userInDocuMentReview == null || !userInDocuMentReview.Any())
            {
                result.AssignedUsers = new List<UserInfoDTO>();
            }
            var filteredUsers = infoUser
                .Where(user => userInDocuMentReview.Any(udr => udr.UserId == user.UserId))
                .ToList();
            var mappedUsers = _mapper.Map<List<UserInfoDTO>>(filteredUsers);

            foreach (var user in mappedUsers)
            {
                var userRoles2 = roles.Where(x => x.UserId == user.UserId).ToList();
                user.Roles = _mapper.Map<List<RoleDTO>>(userRoles2);
            }
            result.AssignedUsers = mappedUsers;

            var userInDocuMentReviewIsActiveSMS = await _documentReviewRepository.GetMulti(x => x.DocId == id && x.IsActiveSMS == true);


            var (infoUser1, roles1) = await _userRepository.GetAllUserWithRolesAsync();

            if (userInDocuMentReviewIsActiveSMS == null || !userInDocuMentReviewIsActiveSMS.Any() || infoUser == null)
            {
                result.SMSUsers = new List<UserInfoDTO>();
            }

            var matchedSMSUsers = infoUser1
                .Where(u => userInDocuMentReviewIsActiveSMS.Any(sms => sms.UserId == u.UserId))
                .ToList();


            var mappedSMSUsers = _mapper.Map<List<UserInfoDTO>>(matchedSMSUsers);


            foreach (var user in mappedSMSUsers)
            {
                var userRoles = roles1.Where(x => x.UserId == user.UserId).ToList();
                user.Roles = _mapper.Map<List<RoleDTO>>(userRoles);
            }

            result.SMSUsers = mappedSMSUsers;


            var userInDocuMentReviewIsAssigned = await _documentReviewRepository.FirstOrDefaultAsync(x => x.DocId == id && x.IsAssigned == true);

            switch (result.StatusCode)
            {
                case AppDocumentStatuses.XIN_Y_KIEN:
                    result.IsRevokeEnabled = (userInDocuMentReview.Any(x => x.UserId.ToString() == assigneeId)) || result.CreatedBy.ToString() == assigneeId;
                    break;

                case AppDocumentStatuses.PHE_DUYET:
                    result.IsRevokeEnabled = (result.CreatedBy.ToString() == assigneeId || userInDocuMentReviewIsAssigned.UserId.ToString() == assigneeId || userInDocuMentReviewIsAssigned.CreatedBy.ToString() == assigneeId);
                    break;
                case AppDocumentStatuses.KY_SO:
                    result.IsRevokeEnabled = (userInDocuMentReviewIsAssigned.UserId.ToString() == assigneeId || userInDocuMentReviewIsAssigned.CreatedBy.ToString() == assigneeId);
                    break;
                default:
                    result.IsRevokeEnabled = false;
                    break;
            }
            var activeStatusIds = new HashSet<int> { 3, 4, 5, 6, 7 };
            var state = await _statusRepository.FirstOrDefaultAsync(s => s.StatusCode == result.StatusCode);
            var workflow = await _workflowRepository.FirstOrDefaultAsync(x => x.StatusId == state.Id);
            var workflows = await _workflowRepository.GetAll()
                .Where(x => activeStatusIds.Contains((int)x.StatusId))
                .Select(x => x.Id)
                .ToListAsync();
            var workflowUsers = await _workflowUserRepository.GetAll()
                .Where(x => workflows.Contains(x.WorkflowId))
                .ToListAsync();
            bool isAssigneeInWorkflowUsers = workflowUsers
      .Any(wu => wu.UserId.ToString().Equals(assigneeId, StringComparison.OrdinalIgnoreCase));
            var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.UserId.ToString() == assigneeId);
            var userRoless = await GetRolesByUserId(currentUser.UserId);
            var department = await _departmentRepository.FirstOrDefaultAsync(d => d.Id == currentUser.DepartmentId);
            var allowedRoles = new HashSet<string>{   
                AppRoleNames.PHO_BI_THU, 
                AppRoleNames.BI_THU,
                AppRoleNames.CHANH_VAN_PHONG, 
                AppRoleNames.PHO_CHANH_VAN_PHONG
            };
           
            bool hasAllowedRole = userRoless.Any(role => allowedRoles.Contains(role));
            bool isCreatedByCurrentUser = data.CreatedBy.ToString() == assigneeId;
            var currentStatus = data.StatusCode;
            bool isXinYKien = currentStatus == AppDocumentStatuses.XIN_Y_KIEN;

            result.IsHistoryTabActive = isCreatedByCurrentUser || isAssigneeInWorkflowUsers || hasAllowedRole || isXinYKien;
            if ((state.Id == 6 || state.Id == 7) &&
                (userInDocuMentReviewIsAssigned.UserId.ToString() == assigneeId || result.CreatedBy.ToString() == assigneeId || hasAllowedRole|| department.DepartmentName == "Tổng Hợp"))
            {
                result.IsHistoryTabActive = true;
            }

            return result;
        }

        public async Task<IEnumerable<DocumentFileDTO>> GetDocumentAttachments(int documentId)
        {
            var fileData = (await _documentFileRepository.GetMulti(x => x.DocId == documentId && x.FileType != -1 && x.FileType.HasValue && x.Deleted == false))
                .OrderByDescending(x => x.Version).ToList();
            if (fileData != null && fileData.Count > 0)
            {
                return _mapper.Map<List<DocumentFileDTO>>(fileData);
            }

            return new List<DocumentFileDTO>();
        }

        public async Task<IEnumerable<DocumentApprovalGroupedDTO>> GetDocumentApprovals(int documentId)
        {
            var approvalData = await _documentReviewRepository.GetMulti(x =>
                x.DocId == documentId && x.SubmitCount.HasValue && x.SubmitCount.Value > 0);

            if (approvalData == null || approvalData.Count == 0)
                return Enumerable.Empty<DocumentApprovalGroupedDTO>();

            var userIds = approvalData.Select(x => x.UserId).Distinct().ToList();

            var validApprovers = await _userRepository.GetMulti(x =>
                userIds.Contains(x.UserId) && !x.IsLockedout);
            var unlockedUserIds = validApprovers.Select(x => x.UserId).ToHashSet();
            var approverDict = validApprovers.ToDictionary(x => x.UserId, x => x.UserFullName);
            var filteredApprovalData = approvalData.Where(x => unlockedUserIds.Contains((Guid)x.UserId)).ToList();
            var approvals = _mapper.Map<List<DocumentReviewDTO>>(filteredApprovalData);

            foreach (var item in approvals)
            {
                if (item.UserId != null && approverDict.TryGetValue(item.UserId.Value, out var fullName))
                {
                    item.UserName = fullName;
                }

                item.Title = item.ReviewResult switch
                {
                    0 => "Không đồng ý",
                    1 => "Đồng ý",
                    2 => "Ý kiến khác",
                    _ => ""
                };
            }

            var fileData = await _documentFileRepository.GetMulti(x =>
                x.DocId == documentId && x.FileType == 2);

            var groupedApprovals = approvals
                .GroupBy(x => x.SubmitCount)
                .OrderBy(g => g.Key)
                .Select(g => new DocumentApprovalGroupedDTO
                {
                    SubmitCount = g.Key,
                    Approvers = g.Select(a => new DocumentApprovalDetailDTO
                    {
                        UserId = (Guid)a.UserId,
                        UserName = a.UserName,
                        Decision = a.Title,
                        CreatedAt = a.Created ?? DateTime.Now,
                        Comment = a.Comment ?? "",
                        Files = fileData
                            .Where(f => f.UserId == a.UserId)
                            .Select(f => new DocumentFileDTO
                            {
                                Id = f.Id,
                                FileName = f.FileName,
                                FilePath = f.FilePath,
                                FilePathToView = f.FilePathToView,
                                IsFinal = f.IsFinal,
                                FileType = f.FileType,
                                DocId = f.DocId,
                                Version = f.Version,
                                UserId = f.UserId,
                                Modified = f.Modified,
                                Deleted = f.Deleted,
                                ModifiedBy = f.ModifiedBy,
                                CreatedBy = f.CreatedBy,
                                Created = f.Created
                            }).ToList()
                    }).ToList(),
                    TotalApprovers = g.Count(),
                    ReviewedCount = g.Count(a => a.ReviewResult != null)
                });

            return groupedApprovals;
        }
        public async Task<DocumentDTO> CreateAndSend(DocumentDTO payload)
        {
            return await CreateDocument(payload, true);
        }

        public async Task<DocumentDTO> CreateDraft(DocumentDTO payload)
        {
            return await CreateDocument(payload, false);
        }

        private async Task<DocumentDTO> CreateDocument(DocumentDTO payload, bool isReadyToSend)
        {
            var documentData = _mapper.Map<TblDocument>(payload);
            if (isReadyToSend)
            {
                documentData.StatusCode = AppDocumentStatuses.XIN_Y_KIEN;
                documentData.SubmitCount = 1;
            }
            else
            {
                documentData.StatusCode = AppDocumentStatuses.DU_THAO;
                documentData.SubmitCount = 0;
                documentData.AssigneeId = documentData.CreatedBy;
            }

            try
            {
                documentData.Created = DateTime.Now;
                documentData.Modified = DateTime.Now;
                await _documentRepository.AddAsync(documentData);
                await _documentRepository.SaveChanges();

                if (payload.MainFiles != null)
                {
                    foreach (var file in payload.MainFiles)
                    {
                        if (file.Length > 0)
                        {
                            var filePathResult = await UploadFile(file);

                            var documentFile = new TblDocumentFile
                            {
                                Id = 0,
                                FileName = file.FileName,
                                FilePath = filePathResult.Item1,
                                FilePathToView = filePathResult.Item2,
                                DocId = documentData.Id,
                                UserId = payload.ModifiedBy,
                                Modified = DateTime.Now,
                                Created = DateTime.Now,
                                Version = 1, //documentData.SubmitCount,
                                Deleted = false,
                                CreatedBy = payload.ModifiedBy,
                                ModifiedBy = payload.ModifiedBy,
                                IsFinal = false,
                                FileType = 1
                            };

                            _documentFileRepository.Add(documentFile);
                            await _documentFileRepository.SaveChanges();
                        }
                    }
                }

                if (payload.SideFiles != null)
                {
                    foreach (var file in payload.SideFiles)
                    {
                        if (file.Length > 0)
                        {
                            var filePathResult = await UploadFile(file);

                            var documentFile = new TblDocumentFile
                            {
                                Id = 0,
                                FileName = file.FileName,
                                FilePath = filePathResult.Item1,
                                FilePathToView = filePathResult.Item2,
                                DocId = documentData.Id,
                                UserId = payload.ModifiedBy,
                                Modified = DateTime.Now,
                                Created = DateTime.Now,
                                Version = 1, //documentData.SubmitCount,
                                Deleted = false,
                                CreatedBy = payload.ModifiedBy,
                                ModifiedBy = payload.ModifiedBy,
                                IsFinal = false,
                                FileType = 0
                            };

                            _documentFileRepository.Add(documentFile);
                            await _documentFileRepository.SaveChanges();
                        }
                    }
                }

                if (isReadyToSend)
                {
                    var historyData = new DocumentHistoryDTO()
                    {
                        DocumentId = documentData.Id,
                        DocumentTitle = documentData.Title,
                        Note = "Tờ trình được gửi lần đầu",
                        Comment = "Trình xin ý kiến",
                        Created = DateTime.Now,
                        CreatedBy = documentData.CreatedBy,
                        DocumentStatus = _statusRepository
                            .FirstOrDefault(s => s.StatusCode == AppDocumentStatuses.XIN_Y_KIEN).Id,
                    };
                    var newHistory = await _historyService.Create(historyData);
                    var currentDocument = _documentRepository.FirstOrDefault(d => d.Id == documentData.Id);
                    currentDocument.CurrentDocumentHistoricalId = newHistory.Id;

                    _documentRepository.Update(currentDocument);
                    await _documentRepository.SaveChanges();

                    if (payload.Users != null && payload.Users.Count > 0)
                    {
                        var reviewItems = new List<TblDocumentReview>();
                        foreach (var userId in payload.Users)
                        {
                            reviewItems.Add(new TblDocumentReview()
                            {
                                DocId = documentData.Id,
                                UserId = userId,
                                DocumentHistoryId = newHistory.Id,
                                //Comment = null,
                                SubmitCount = 1,
                                //FilePath = null,
                                //ReviewResult = null,
                                //Title = null,
                                Viewed = false,
                                Deleted = false,
                                IsAssigned = true,
                                Modified = DateTime.Now,
                                Created = DateTime.Now,
                                CreatedBy = documentData.CreatedBy,
                            });
                        }

                        await _documentReviewRepository.AddRangeAsync(reviewItems);
                        await _documentReviewRepository.SaveChanges();
                    }
                    if (payload.UsersSMS != null && payload.UsersSMS.Count > 0)
                    {
                        var reviewItems = new List<TblDocumentReview>();
                        foreach (var userId in payload.UsersSMS)
                        {
                            reviewItems.Add(new TblDocumentReview()
                            {
                                DocId = documentData.Id,
                                UserId = userId,
                                DocumentHistoryId = newHistory.Id,
                                //Comment = null,
                                SubmitCount = null,
                                //FilePath = null,
                                //ReviewResult = null,
                                //Title = null,
                                Viewed = false,
                                Deleted = false,
                                IsAssigned = false,
                                Modified = DateTime.Now,
                                Created = DateTime.Now,
                                CreatedBy = documentData.CreatedBy,
                                IsActiveSMS = true
                            });
                        }

                        await _documentReviewRepository.AddRangeAsync(reviewItems);
                        await _documentReviewRepository.SaveChanges();
                    }
                }
                else {
                    var historyData = new DocumentHistoryDTO()
                    {
                        DocumentId = documentData.Id,
                        DocumentTitle = documentData.Title,
                        Note = "Tờ trình đang trong giai đoạn soạn thảo",
                        Comment = "Soạn thảo nội dung",
                        Created = DateTime.Now,
                        CreatedBy = documentData.CreatedBy,
                        DocumentStatus = _statusRepository
                          .FirstOrDefault(s => s.StatusCode == AppDocumentStatuses.DU_THAO).Id,
                    };
                    var newHistory = await _historyService.Create(historyData);
                    var currentDocument = _documentRepository.FirstOrDefault(d => d.Id == documentData.Id);
                    currentDocument.CurrentDocumentHistoricalId = newHistory.Id;

                    _documentRepository.Update(currentDocument);
                    await _documentRepository.SaveChanges();

                    if (payload.Users != null && payload.Users.Count > 0)
                    {
                        var reviewItems = new List<TblDocumentReview>();
                        foreach (var userId in payload.Users)
                        {
                            reviewItems.Add(new TblDocumentReview()
                            {
                                DocId = documentData.Id,
                                UserId = userId,
                                DocumentHistoryId = newHistory.Id,
                                //Comment = null,
                                SubmitCount = 1,
                                //FilePath = null,
                                //ReviewResult = null,
                                //Title = null,
                                Viewed = false,
                                Deleted = false,
                                IsAssigned = false,
                                Modified = DateTime.Now,
                                Created = DateTime.Now,
                                CreatedBy = documentData.CreatedBy,
                            });
                        }

                        await _documentReviewRepository.AddRangeAsync(reviewItems);
                        await _documentReviewRepository.SaveChanges();
                    }
                    if (payload.UsersSMS != null && payload.UsersSMS.Count > 0)
                    {
                        var reviewItems = new List<TblDocumentReview>();
                        foreach (var userId in payload.UsersSMS)
                        {
                            reviewItems.Add(new TblDocumentReview()
                            {
                                DocId = documentData.Id,
                                UserId = userId,
                                DocumentHistoryId = newHistory.Id,
                                //Comment = null,
                                SubmitCount = null,
                                //FilePath = null,
                                //ReviewResult = null,
                                //Title = null,
                                Viewed = false,
                                Deleted = false,
                                IsAssigned = false,
                                Modified = DateTime.Now,
                                Created = DateTime.Now,
                                CreatedBy = documentData.CreatedBy,
                                IsActiveSMS = true
                            });
                        }

                        await _documentReviewRepository.AddRangeAsync(reviewItems);
                        await _documentReviewRepository.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _mapper.Map<DocumentDTO>(documentData);
        }

        public async Task<ChartRequests> GetDocumentsByCharMonth()
        {
            // Assuming _documentRepository.GetDocumentCountByMonth(month) is the method to get counts of documents per month.
            List<int> documentCounts = new List<int>();
            for (int month = 1; month <= 12; month++)
            {
                int count = await _documentRepository.GetDocumentCountByMonth(month);
                documentCounts.Add(count);
            }

            DatasetsChart datasetsChart = new DatasetsChart()
            {
                label = "Tờ trình trong tháng",
                fill = false,
                borderWidth = 2,
                lineTension = 0,
                spanGaps = true,
                borderColor = "#efefef",
                pointRadius = 3,
                pointHoverRadius = 7,
                pointColor = "#efefef",
                pointBackgroundColor = "#efefef",
                data = documentCounts.ToArray() // Assign the counts to the 'data' property
            };

            ChartRequests chart = new ChartRequests()
            {
                labels = new string[]
                {
                    "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9",
                    "Tháng 10", "Tháng 11", "Tháng 12"
                },
                datasets = datasetsChart
            };

            return chart;
        }

        public async Task<List<ChartDataByField>> GetDocumentsByChartField()
        {
            var listFields = _mapper.Map<List<TblField>>(_fieldRepository.GetAll());

            List<ChartDataByField> newChar = new List<ChartDataByField>();

            foreach (var field in listFields)
            {
                newChar.Add(new ChartDataByField()
                {
                    title = field.Title,
                    data = await _documentRepository.GetDocumentCountByField(field.Id),
                });
            }

            return newChar;
        }

        public async Task<DocumentGroup> CountDocumentByStatus()
        {
            var listStatus = _mapper.Map<List<TblStatuses>>(_statusRepository.GetAll());
            List<CountDocumentByStatus> listCount = new List<CountDocumentByStatus>();
            int total = 0;
            foreach (var status in listStatus.Where(s => s.StatusCode != AppDocumentStatuses.DU_THAO))
            {
                int count = await _documentRepository.CountDocumentByStatus(status.StatusCode);
                listCount.Add(new CountDocumentByStatus()
                {
                    Name = status.StatusCode,
                    Description = status.Title,
                    Data = count
                });
                total += count;
            }

            DocumentGroup result = new DocumentGroup()
            {
                Label = "Tổng tờ trình",
                Total = total,
                Records = listCount,
            };
            return result;
        }

        public async Task<pieData> pieData()
        {
            var fieldIdGroups = _documentRepository.Query(x => x.StatusCode != AppDocumentStatuses.DU_THAO)
                .GroupBy(x => x.FieldId)
                .ToList();

            List<int> data = new List<int>();
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();

            var fieldIds = fieldIdGroups.Select(g => g.Key).ToList();

            var fieldTitles = _context.TblFields
                .Where(field => fieldIds.Contains(field.Id))
                .ToDictionary(field => field.Id, field => field.Title);

            foreach (var group in fieldIdGroups)
            {
                int key = group.Key ?? 0;
                if (fieldTitles.TryGetValue(key, out var title))
                {
                    labels.Add(title);
                }
                else
                {
                    labels.Add($"FieldId: {group.Key}");
                }

                data.Add(group.Count());

                // Generating random hex color code
                string color = $"#{new Random().Next(0x1000000):X6}";
                backgroundColor.Add(color);
            }

            var datasetsPie = new List<datasetspie>
            {
                new datasetspie
                {
                    data = data.ToArray(),
                    backgroundColor = backgroundColor.ToArray()
                }
            };

            var pieData = new pieData
            {
                labels = labels.ToArray(),
                datasets = datasetsPie
            };

            return pieData;
        }

        public async Task<IEnumerable<DocumentDTO>> GetAll()
        {
            return new List<DocumentDTO>();
        }

        public async Task<int> UpdateDocumentStatus(int docId, string status)
        {
            try
            {
                var document = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);
                document.StatusCode = "cho-ban-hanh";
                _documentRepository.Update(document);
                await _documentRepository.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> DeleteDocument(int docId)
        {
            try
            {
                var document = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);
                if (document != null)
                {
                    _documentRepository.Remove(document);
                    await _documentRepository.SaveChanges();
                    return document.Id;
                }
                else throw new Exception("Xóa tờ trình không thành công - Không tìm thấy tờ trình");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<int> RemoveDocumentFiles(int docId)
        {
            var documentFiles = await _documentFileRepository.GetMulti(x => x.DocId == docId);
            if (documentFiles.Count == 0) return 0;

            var res = 0;
            foreach (var file in documentFiles)
            {
                var res1 = await RemoveFileByPath(file.FilePath);
                var res2 = await RemoveFileByPath(file.FilePathToView);

                res = res + (res1 && res2 ? 1 : 0);
            }

            return res;
        }

        public async Task<int> UpdatePriorityDocument(int docId, int priorityNumber)
        {
            try
            {
                var document = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);
                if (document != null)
                {
                    document.PriorityNumber = priorityNumber;
                    _documentRepository.Update(document);
                    await _documentRepository.SaveChanges();
                    return document.Id;
                }
                else throw new Exception("Cập nhật độ ưu tiên tờ trình không thành công - Không tìm thấy tờ trình");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> SignedFile(int docId, string userId)
        {
            var fullTemplatePath = Path.Combine("wwwroot", @"Files\Mau ký số từng người.docx");
            var newDirectory = Path.Combine("wwwroot", @"Files\Document_Attachments");
            var userName = await GetNameUser(userId);
            var newFilePath = Path.Combine(newDirectory, $"{docId}_Mau_ky.docx");
            try
            {
                var doc = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);
                var reviewProcessDetail =
                    await _documentReviewRepository.FirstOrDefaultAsync(x =>
                        x.DocId == docId && x.UserId == new Guid(userId));
                var currentDate = DateTime.Now;

                var field = await _fieldRepository.FirstOrDefaultAsync(x => x.Id == doc.FieldId);
                File.Copy(fullTemplatePath, newFilePath);
                using (var wordProcess = WordprocessingDocument.Open(newFilePath, true))
                {
                    ReplacePlaceHolder(wordProcess, "TITLE", doc.Title ?? "");
                    ReplacePlaceHolder(wordProcess, "dd", currentDate.Day.ToString());
                    ReplacePlaceHolder(wordProcess, "mm", currentDate.Month.ToString());
                    ReplacePlaceHolder(wordProcess, "yyyy", currentDate.Year.ToString());
                    ReplacePlaceHolder(wordProcess, "CHUYEN_VIEN_TRINH",
                        await GetFullNameUser(doc.CreatedBy.ToString()));
                    ReplacePlaceHolder(wordProcess, "LINH_VUC", field.Title);
                    ReplacePlaceHolder(wordProcess, "CREATED", doc.Created.Value.ToString("dd/MM/yyyy"));
                    ReplacePlaceHolder(wordProcess, "DATE_END", doc.DateEndApproval.Value.ToString("dd/MM/yyyy"));
                    ReplacePlaceHolder(wordProcess, "NAME", await GetFullNameUser(userId));
                    ReplacePlaceHolder(wordProcess, "STATUS", GetReviewStatus(reviewProcessDetail.ReviewResult.Value));
                    ReplacePlaceHolder(wordProcess, "COMMENT", reviewProcessDetail.Comment);
                    wordProcess.Save();
                }

                var file = ToIFormFileWord(newFilePath,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document", userName);
                var documentDTO = new DocumentDTO
                {
                    Id = docId,
                    ModifiedBy = new Guid(userId),
                    CreatedBy = new Guid(userId)
                };

                var fullPathFile = await SaveFileSigned(file, documentDTO, 2);

                File.Delete(newFilePath);
                return new
                {
                    Error = false,
                    linkPdfToView = fullPathFile.FilePathToView ?? ""
                };
            }
            catch (Exception ex)
            {
                File.Delete(newFilePath);
                return new
                {
                    Error = true,
                    linkPdfToView = ""
                };
                throw new Exception(ex.Message);
            }
        }

        public async Task<byte[]> PrintResult(int docId, string userId, string comment, string currentUrl)
        {
            var fullTemplatePath = Path.Combine("wwwroot", @"Files\KetQuaCVP.docx");
            var newDirectory = Path.Combine("wwwroot", @"Files\Document_Attachments");
            var newFilePath = Path.Combine(newDirectory, $"{docId}_KetQua.docx");

            try
            {
                if (!File.Exists(fullTemplatePath))
                {
                    throw new FileNotFoundException("Template file not found", fullTemplatePath);
                }

                var doc = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);
                if (doc == null)
                {
                    throw new ArgumentNullException(nameof(doc), "Document not found");
                }

                var docReviews = await _documentReviewRepository.GetMulti(x => x.DocId == docId && x.ReviewResult.HasValue && x.SubmitCount.HasValue);
                List<string[]> dataRows = new List<string[]>();
                var i = 1;

                foreach (var docReview in docReviews)
                {
                    var user = await _userRepository.FirstOrDefaultAsync(u => u.UserId == docReview.UserId && u.IsLockedout != true);
                    if (user == null) continue;
                    var userInRole = await _userInRoleRepository.FirstOrDefaultAsync(m => m.UserId == docReview.UserId);
                    var filesReviewers = await _documentFileRepository.FirstOrDefaultAsync(m => m.DocId == docId && m.UserId == docReview.UserId && m.FileType == 2);
                    var hostUrl = _configuration["AppSettings:HostURL"];
                    string link = filesReviewers != null
                        ? $"{hostUrl}/admin/to-trinh/{doc.StatusCode}/document-detail/{docId}?assigneeId={userId}&fileName=file&userId={docReview.UserId.ToString()}"
                        : "";

                    var userName = await GetFullNameUser(docReview.UserId.ToString());
                    string[] data;
                    if (docReview.ReviewResult == 0)
                    {
                        data = new string[] { $"{i++}", $"{userName ?? ""}", "", "X", "", docReview.Comment, $"{link}" };
                    }
                    else if (docReview.ReviewResult == 1)
                    {
                        data = new string[] { $"{i++}", $"{userName ?? ""}", "X", "", "", docReview.Comment, $"{link}" };
                    }
                    else
                    {
                        data = new string[] { $"{i++}", $"{userName ?? ""}", "", "", "X", docReview.Comment, $"{link}" };
                    }

                    dataRows.Add(data);
                }
                var currentDate = DateTime.Now;

                var field = await _fieldRepository.FirstOrDefaultAsync(x => x.Id == doc.FieldId);
                File.Copy(fullTemplatePath, newFilePath, true);
                using (var wordProcess = WordprocessingDocument.Open(newFilePath, true))
                {
                    ReplacePlaceHolder(wordProcess, "TITLE", doc.Title ?? "");
                    ReplacePlaceHolder(wordProcess, "dd", currentDate.Day.ToString());
                    ReplacePlaceHolder(wordProcess, "mm", currentDate.Month.ToString());
                    ReplacePlaceHolder(wordProcess, "yyyy", currentDate.Year.ToString());
                    ReplacePlaceHolder(wordProcess, "CHUYEN_VIEN_TRINH",
                        await GetFullNameUser(doc.CreatedBy.ToString()));
                    ReplacePlaceHolder(wordProcess, "LINH_VUC", field.Title);
                    ReplacePlaceHolder(wordProcess, "CREATED", doc.Created.Value.ToString("dd/MM/yyyy"));
                    ReplacePlaceHolder(wordProcess, "DATE_END", doc.DateEndApproval.Value.ToString("dd/MM/yyyy"));
                    ReplacePlaceholderWithCustomTable(wordProcess, "TABLE", dataRows);
                    ReplacePlaceHolder(wordProcess, "COMMENT", comment ?? "");
                    wordProcess.Save();
                }

                if (!File.Exists(newFilePath))
                {
                    throw new FileNotFoundException("Generated file not found", newFilePath);
                }

                byte[] fileBytes = File.ReadAllBytes(newFilePath);
                File.Delete(newFilePath);
                return fileBytes;
            }
            catch (Exception ex)
            {
                if (File.Exists(newFilePath))
                {
                    File.Delete(newFilePath);
                }
                throw;
            }
        }

        public async Task<object> ResultSignedFiles(int docId, string userId, int submitCount)
        {
            var fullTemplatePath = Path.Combine("wwwroot", @"Files\KetQua.docx");
            var newDirectory = Path.Combine("wwwroot", @"Files\Document_Attachments");
            var currentDate = DateTime.Now;
            //var formattedDate = currentDate.ToString("yyyyMMddHHmmss");
            string newFilePath = "";

            try
            {
                var doc = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);
                var docReviews = await _documentReviewRepository.GetMulti(x => x.DocId == docId && x.SubmitCount == submitCount);
                var approvalCount = docReviews.Count();

                newFilePath = Path.Combine(newDirectory, $"{docId}_KetQua_LanTrinh{submitCount}.docx");
                List<string[]> dataRows = new List<string[]>();
                var i = 1;

                foreach (var docReview in docReviews)
                {
                    var user = await _userRepository.FirstOrDefaultAsync(u => u.UserId == docReview.UserId && u.IsLockedout != true);
                    if (user == null) continue;
                    var userInRole = await _userInRoleRepository.FirstOrDefaultAsync(m => m.UserId == docReview.UserId);
                    //var role = await _roleRepository.FirstOrDefaultAsync(m => m.RoleId == userInRole.RoleId && m.RoleName == "chuyen-vien");
                    //if (role != null) continue;

                    var userName = await GetFullNameUser(docReview.UserId.ToString());
                    var filesReviewers =
                      await _documentFileRepository.FirstOrDefaultAsync(m => m.DocId == docId && m.UserId == docReview.UserId && m.FileType == 2);
                    var hostUrl = _configuration["AppSettings:HostURL"];
                    string link = filesReviewers != null
                        ? $"{hostUrl}/admin/to-trinh/{doc.StatusCode}/document-detail/{docId}?assigneeId={userId}&fileName=file&userId={docReview.UserId.ToString()}"
                        : "";
                    string[] data = docReview.ReviewResult switch
                    {
                        null => new string[] { $"{i++}", $"{userName ?? ""}", "", "", "", docReview.Comment ?? "", $"{link}" },
                        0 => new string[] { $"{i++}", $"{userName ?? ""}", "", "X", "", docReview.Comment ?? "", $"{link}" },
                        1 => new string[] { $"{i++}", $"{userName ?? ""}", "X", "", "", docReview.Comment ?? "", $"{link}" },
                        2 => new string[] { $"{i++}", $"{userName ?? ""}", "", "", "X", docReview.Comment ?? "", $"{link}" },
                        _ => new string[] { $"{i++}", $"{userName ?? ""}", "", "", "", docReview.Comment ?? "", $"{link}" },
                    };

                    dataRows.Add(data);
                }
                var field = await _fieldRepository.FirstOrDefaultAsync(x => x.Id == doc.FieldId);
                File.Copy(fullTemplatePath, newFilePath, true);
                using (var wordProcess = WordprocessingDocument.Open(newFilePath, true))
                {
                    ReplacePlaceHolderSubmitCount(wordProcess, "COUNT", submitCount.ToString() ?? "");
                    ReplacePlaceHolder(wordProcess, "TITLE", doc.Title ?? "");
                    ReplacePlaceHolder(wordProcess, "dd", currentDate.Day.ToString());
                    ReplacePlaceHolder(wordProcess, "mm", currentDate.Month.ToString());
                    ReplacePlaceHolder(wordProcess, "yyyy", currentDate.Year.ToString());
                    ReplacePlaceHolder(wordProcess, "CHUYEN_VIEN_TRINH", await GetFullNameUser(doc.CreatedBy.ToString()));
                    ReplacePlaceHolder(wordProcess, "LINH_VUC", field.Title ?? "");
                    ReplacePlaceHolder(wordProcess, "CREATED", doc.Created?.ToString("dd/MM/yyyy") ?? "");
                    ReplacePlaceHolder(wordProcess, "DATE_END", doc.DateEndApproval?.ToString("dd/MM/yyyy") ?? "");
                    ReplacePlaceholderWithCustomTable(wordProcess, "TABLE", dataRows);
                    wordProcess.Save();
                }

                var documentFile = new TblDocumentFile
                {
                    Id = 0,
                    FileName = Path.GetFileName(newFilePath),
                    FilePath = $"/Files/Document_Attachments/{Path.GetFileName(newFilePath)}",
                    FilePathToView = $"/Files/Document_Attachments/{Path.GetFileName(newFilePath)}",
                    DocId = docId,
                    UserId = new Guid(userId),
                    Modified = DateTime.Now,
                    Created = DateTime.Now,
                    Version = submitCount,
                    Deleted = false,
                    CreatedBy = new Guid(userId),
                    ModifiedBy = new Guid(userId),
                    IsFinal = false,
                    FileType = -1
                };
                await _documentFileRepository.AddAsync(documentFile);
                await _documentFileRepository.SaveChanges();

                return new
                {
                    Error = false,
                    linkZipToView = documentFile.FilePathToView ?? ""
                };
            }
            catch (Exception ex)
            {
                if (File.Exists(newFilePath)) File.Delete(newFilePath);

                return new
                {
                    Error = true,
                    Message = ex.Message
                };
            }
        }
        public string GetReviewStatus(int reviewStatus)
        {
            if (reviewStatus == 0)
            {
                return "Không đồng ý";
            }
            else if (reviewStatus == 1)
            {
                return "Đồng ý";
            }
            else
            {
                return "Ý kiến khác";
            }
        }

        public async Task<string> GetNameUser(string userId)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == new Guid(userId));

            return user?.UserName ?? string.Empty;
        }

        public async Task<string> GetFullNameUser(string userId)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == new Guid(userId));

            return user?.UserFullName ?? string.Empty;
        }

        public static IFormFile ToIFormFileWord(string filePath, string extension, string userName)
        {
            var replaceText = $"_{userName}.docx";
            if (userName == "")
            {
                replaceText = ".docx";
            }

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var fileBytes = new byte[stream.Length];
                stream.Read(fileBytes, 0, (int)stream.Length);

                var copyForFormFile = new byte[fileBytes.Length];
                Array.Copy(fileBytes, copyForFormFile, fileBytes.Length);

                var fileInfo = new FileInfo(filePath);

                var formFile = new FormFile(
                    new MemoryStream(copyForFormFile),
                    0,
                    fileBytes.Length,
                    fileInfo.Name.Replace(".docx", replaceText),
                    fileInfo.Name.Replace(".docx", replaceText)
                )
                {
                    Headers = new HeaderDictionary(),
                    ContentType = extension
                };

                return formFile;
            }
        }

        public static void ReplacePlaceholderWithCustomTable(WordprocessingDocument document, string placeholderText, List<string[]> data)
        {
            Body body = document.MainDocumentPart.Document.Body;
            IEnumerable<Run> runs = body.Descendants<Run>();
            Run placeholderRun = runs.FirstOrDefault(r => r.InnerText == placeholderText);

            string[] headers = { "STT", "Họ và Tên", "Đồng Ý", "Không đồng ý", "Có ý kiến khác", "Nội dung ý kiến", "Ghi chú" };
            int[] columnWidths = { 300, 2409, 992, 992, 992, 2409, 992 };

            Table table = new Table();

            TableProperties tableProperties = new TableProperties(
                new TableWidth { Width = "100%", Type = TableWidthUnitValues.Pct },
                new TableBorders
                {
                    TopBorder = new TopBorder { Val = BorderValues.Single, Color = "000000" },
                    BottomBorder = new BottomBorder { Val = BorderValues.Single, Color = "000000" },
                    LeftBorder = new LeftBorder { Val = BorderValues.Single, Color = "000000" },
                    RightBorder = new RightBorder { Val = BorderValues.Single, Color = "000000" },
                    InsideHorizontalBorder = new InsideHorizontalBorder { Val = BorderValues.Single, Color = "000000" },
                    InsideVerticalBorder = new InsideVerticalBorder { Val = BorderValues.Single, Color = "000000" }
                }
            );
            table.AppendChild(tableProperties);

            TableGrid tableGrid = new TableGrid();
            foreach (int width in columnWidths)
            {
                tableGrid.Append(new GridColumn { Width = width.ToString() });
            }
            table.AppendChild(tableGrid);

            // Header Row
            TableRow headerRow = new TableRow();
            for (int i = 0; i < headers.Length; i++)
            {
                TableCell cell = new TableCell();
                cell.Append(new TableCellProperties(
                    new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = columnWidths[i].ToString() },
                    new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                ));

                Run run = new Run(new Text(headers[i]));
                // Apply font size and font type
                run.PrependChild(new RunProperties(
                    new Bold(),
                    new FontSize { Val = "26" }, // 13pt
                    new RunFonts { Ascii = "Times New Roman" }
                ));

                Paragraph paragraph = new Paragraph(run)
                {
                    ParagraphProperties = new ParagraphProperties(
                        new Justification { Val = JustificationValues.Center },
                        new SpacingBetweenLines
                        {
                            Before = "0",
                            After = "0",
                            Line = "240", // 1.5 line spacing (optional)
                            LineRule = LineSpacingRuleValues.Auto
                        }
                    )
                };

                cell.Append(paragraph);
                headerRow.Append(cell);
            }
            table.Append(headerRow);

            // Data Rows
            foreach (string[] row in data)
            {
                TableRow dataRow = new TableRow();
                for (int i = 0; i < row.Length; i++)
                {
                    TableCell cell = new TableCell();
                    cell.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = columnWidths[i].ToString() },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                    ));

                    string cellValue = row[i];
                    Paragraph paragraph = new Paragraph();

                    
                    RunProperties runProps = new RunProperties(
                        new FontSize { Val = "26" }, // 13pt
                        new RunFonts { Ascii = "Times New Roman" } 
                    );

                    if (i == 6 && !string.IsNullOrEmpty(cellValue)) 
                    {
                        HyperlinkRelationship hyperlinkRel = document.MainDocumentPart.AddHyperlinkRelationship(new Uri(cellValue, UriKind.Absolute), true);
                        string relationshipId = hyperlinkRel.Id;

                        Run hyperlinkRun = new Run(new Text("Link file"));
                        hyperlinkRun.PrependChild(new RunProperties(
                            new FontSize { Val = "26" }, // 13pt
                            new RunFonts { Ascii = "Times New Roman" }, 
                            new Color { Val = "0000FF" },
                            new Underline { Val = UnderlineValues.Single }
                        ));

                        Hyperlink hyperlink = new Hyperlink(hyperlinkRun) { Id = relationshipId };
                        paragraph.Append(hyperlink);
                    }
                    else
                    {
                        
                        Run dataRun = new Run(new Text(cellValue));
                        dataRun.PrependChild(runProps);
                        paragraph.Append(dataRun);
                    }

                    
                    JustificationValues alignment = (i == 5) ? JustificationValues.Left : JustificationValues.Center;

                    paragraph.ParagraphProperties = new ParagraphProperties(
                        new Justification { Val = alignment },
                        new SpacingBetweenLines
                        {
                            Before = "0",
                            After = "0",
                            Line = "240", // Line spacing (optional)
                            LineRule = LineSpacingRuleValues.Auto
                        }
                    );

                    cell.Append(paragraph);
                    dataRow.Append(cell);
                }
                table.Append(dataRow);
            }

            Paragraph placeholderParagraph = (Paragraph)placeholderRun.Parent;
            placeholderParagraph.InsertAfterSelf(table);
            placeholderRun.Remove();
        }

        public static void ReplacePlaceHolder(WordprocessingDocument document, string placeHolder,
            string replacementText)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (string.IsNullOrEmpty(placeHolder))
            {
                throw new ArgumentException("PlaceHolder cannot be null or empty.", nameof(placeHolder));
            }

            // Find all Text elements in the document
            var allTextElements = document.MainDocumentPart.Document.Descendants<Text>();

            foreach (var textElement in allTextElements)
            {
                if (textElement.Text.Equals(placeHolder))
                {
                    textElement.Text = replacementText;
                }
            }
        }
        public static void ReplacePlaceHolderSubmitCount(WordprocessingDocument document, string placeHolder, string replacementText)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            if (string.IsNullOrEmpty(placeHolder))
            {
                throw new ArgumentException("PlaceHolder cannot be null or empty.", nameof(placeHolder));
            }
            var allTextElements = document.MainDocumentPart.Document.Descendants<Text>();
            foreach (var textElement in allTextElements)
            {
                if (textElement.Text.Equals(placeHolder))
                {
                    textElement.Text = replacementText;
                    var runElement = textElement.Parent as Run;
                    if (runElement != null)
                    {
                        var runProperties = runElement.RunProperties ?? new RunProperties();
                        runElement.RunProperties = runProperties;
                        runProperties.Bold = new Bold();
                    }
                }
            }
        }
        private async Task<bool> RemoveFileByPath(string filePath)
        {
            try
            {
                var fullPath = Path.Combine("wwwroot", filePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(filePath);
                    return true;
                }
                else
                {
                    _logger.LogInformation("Không tìm thấy địa chỉ file: " + filePath);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return false;
            }
        }

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
                    string currentDirectory = Directory.GetCurrentDirectory();


                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var fullFilePath = "/Files/Document_Attachments/" + fileName;
                    var sourceFile = currentDirectory + "\\wwwroot\\Files\\Document_Attachments\\" + fileName;
                    var destinationFile = "";
                    var filePathToView = "";
                    var fullFilePathToView = "";
                    var controller = new WordToPdfController();
                    if (filePath.EndsWith(".doc"))
                    {
                        filePathToView = filePath.Replace(".doc", ".pdf");
                        fullFilePathToView = fullFilePath.Replace(".doc", ".pdf");
                        destinationFile = sourceFile.Replace(".doc", ".pdf");
                        controller.ConvertWordToPdf(sourceFile, destinationFile);
                    }
                    else if (filePath.EndsWith(".docx"))
                    {
                        filePathToView = filePath.Replace(".docx", ".pdf");
                        fullFilePathToView = fullFilePath.Replace(".docx", ".pdf");
                        destinationFile = sourceFile.Replace(".docx", ".pdf");
                        controller.ConvertWordToPdf(sourceFile, destinationFile);
                    }
                    else if (filePath.EndsWith(".xls"))
                    {
                        filePathToView = filePath.Replace(".xls", ".pdf");
                        fullFilePathToView = fullFilePath.Replace(".xls", ".pdf");
                        destinationFile = sourceFile.Replace(".xls", ".pdf");
                        controller.ConvertExcelToPdf(sourceFile, destinationFile);
                    }
                    else if (filePath.EndsWith(".xlsx"))
                    {
                        filePathToView = filePath.Replace(".xlsx", ".pdf");
                        fullFilePathToView = fullFilePath.Replace(".xlsx", ".pdf");
                        destinationFile = sourceFile.Replace(".xlsx", ".pdf");
                        controller.ConvertExcelToPdf(sourceFile, destinationFile);
                    }
                    else if (filePath.EndsWith(".ppt"))
                    {
                        filePathToView = filePath.Replace(".ppt", ".pdf");
                        fullFilePathToView = fullFilePath.Replace(".ppt", ".pdf");
                        destinationFile = sourceFile.Replace(".ppt", ".pdf");
                        controller.ConvertPowerPointToPdf(sourceFile, destinationFile);
                    }
                    else if (filePath.EndsWith(".pptx"))
                    {
                        filePathToView = filePath.Replace(".pptx", ".pdf");
                        fullFilePathToView = fullFilePath.Replace(".pptx", ".pdf");
                        destinationFile = sourceFile.Replace(".pptx", ".pdf");
                        controller.ConvertPowerPointToPdf(sourceFile, destinationFile);
                    }

                    if (fullFilePathToView == "") fullFilePathToView = fullFilePath;

                    return new Tuple<string, string>(fullFilePath, fullFilePathToView);
                }

                return new Tuple<string, string>("", "");
            }
            catch (Exception ex)
            {
                return new Tuple<string, string>("", "");
            }
        }

        public async Task<DocumentDTO> UpdateDocument(DocumentDTO payload, IFormFile[] files)
        {
            try
            {
                var document = await _documentRepository.FirstOrDefaultAsync(x => x.Id == payload.Id);
                if (document != null)
                {
                    bool isBackToPreviousStatus = (document.StatusCode == AppDocumentStatuses.DU_THAO && document.PreviousStatusCode == AppDocumentStatuses.XIN_Y_KIEN) ||
                         (document.StatusCode == AppDocumentStatuses.XIN_Y_KIEN_LAI && document.PreviousStatusCode == AppDocumentStatuses.XIN_Y_KIEN);
                    int targetDocumentHistoryId = (int)document.CurrentDocumentHistoricalId;
                    if (isBackToPreviousStatus)
                    {
                        var previousHistory = (await _documentHistoryRepository
                            .GetMulti(x => x.DocumentId == document.Id && x.Id < document.CurrentDocumentHistoricalId))
                            .OrderByDescending(x => x.Id)
                            .FirstOrDefault();
                        if (previousHistory != null)
                        {
                            targetDocumentHistoryId = previousHistory.Id;
                        }
                    }

                    var approvals = await _documentReviewRepository.GetMulti(x => x.DocId == payload.Id && x.SubmitCount.HasValue && x.DocumentHistoryId == targetDocumentHistoryId);
                    bool isSent = document.StatusCode != AppDocumentStatuses.XIN_Y_KIEN &&
                                (payload.StatusCode == AppDocumentStatuses.XIN_Y_KIEN ||
                                 payload.StatusCode == AppDocumentStatuses.DU_THAO);
                    // remove item when at previousHistory
                    if ((isBackToPreviousStatus && approvals.Any())|| isSent)
                    {
                        _documentReviewRepository.RemoveRange(approvals);
                        await _documentReviewRepository.SaveChanges();
                    }

                    if (isSent) {
                        if(document.PreviousStatusCode == AppDocumentStatuses.PHE_DUYET && document.StatusCode == AppDocumentStatuses.XIN_Y_KIEN_LAI)
                        {
                            document.SubmitCount += 1;
                            document.PreviousStatusCode = AppDocumentStatuses.XIN_Y_KIEN_LAI;
                            var documentReviewData = await _documentReviewRepository.GetMulti(x => x.DocId == payload.Id && x.IsAssigned == true);
                            if (documentReviewData != null && documentReviewData.Any())
                            {
                                foreach (var review in documentReviewData)
                                {
                                    review.IsAssigned = false;
                                }
                                _documentReviewRepository.UpdateRange(documentReviewData);
                                await _documentReviewRepository.SaveChanges();
                            }
                        }
                        else if(document.StatusCode == AppDocumentStatuses.PHE_DUYET)
                        {
                            document.PreviousStatusCode = AppDocumentStatuses.XIN_Y_KIEN;
                        }
                        else if (document.StatusCode == AppDocumentStatuses.DU_THAO && document.PreviousStatusCode == null) 
                        {
                            document.SubmitCount += 1;
                            document.PreviousStatusCode = AppDocumentStatuses.DU_THAO;
                        }
                        else if (document.StatusCode == AppDocumentStatuses.XIN_Y_KIEN_LAI && document.PreviousStatusCode == AppDocumentStatuses.XIN_Y_KIEN)
                        {
                            document.PreviousStatusCode = AppDocumentStatuses.XIN_Y_KIEN_LAI;
                        }
                        else
                        {
                            document.PreviousStatusCode = AppDocumentStatuses.DU_THAO;
                        }    
                    }
                    document.Title = payload.Title;
                    document.Note = payload.Note;
                    document.DateEndApproval = payload.DateEndApproval;
                    document.Modified = DateTime.Now;
                    //document.PreviousStatusCode = document.StatusCode;
                    document.StatusCode = payload.StatusCode;
                    document.FieldId = payload.FieldId;
                    document.TypeId = payload.TypeId;
                    document.RemindDatetime = payload.RemindDatetime;
                    var newHistory = await CreateDocumentHistoryAsync(document);
                    if (newHistory != null)
                        document.CurrentDocumentHistoricalId = newHistory.Id;
                    _documentRepository.Update(document);
                    await _documentRepository.SaveChanges();

                    //if (isSent)
                    //{
                    //    foreach (var approval in approvals)
                    //    {
                    //        approval.Viewed = false;
                    //        approval.IsAssigned = true;
                    //        approval.Modified = DateTime.Now;
                    //        approval.DocumentHistoryId = newHistory.Id;
                    //        _documentReviewRepository.Update(approval);
                    //        await _documentReviewRepository.SaveChanges();
                    //    }
                    //}

                    if (payload.MainFiles != null)
                    {
                        foreach (var file in payload.MainFiles)
                        {
                            if (file.Length > 0)
                            {
                                var filePathResult = await UploadFile(file);

                                var documentFile = new TblDocumentFile
                                {
                                    Id = 0,
                                    FileName = file.FileName,
                                    FilePath = filePathResult.Item1,
                                    FilePathToView = filePathResult.Item2,
                                    DocId = document.Id,
                                    UserId = payload.ModifiedBy,
                                    Modified = DateTime.Now,
                                    Created = DateTime.Now,
                                    Version =
                                        0, //payload.StatusCode == 3 || payload.StatusCode == 4 ? document.SubmitCount : document.SubmitCount + 1,
                                    Deleted = false,
                                    CreatedBy = payload.ModifiedBy,
                                    ModifiedBy = payload.ModifiedBy,
                                    IsFinal = payload.StatusCode == AppDocumentStatuses.BAN_HANH,
                                    FileType = 1
                                };

                                _documentFileRepository.Add(documentFile);
                                await _documentFileRepository.SaveChanges();
                            }
                        }
                    }

                    if (payload.SideFiles != null)
                    {
                        foreach (var file in payload.SideFiles)
                        {
                            if (file.Length > 0)
                            {
                                var filePathResult = await UploadFile(file);

                                var documentFile = new TblDocumentFile
                                {
                                    Id = 0,
                                    FileName = file.FileName,
                                    FilePath = filePathResult.Item1,
                                    FilePathToView = filePathResult.Item2,
                                    DocId = document.Id,
                                    UserId = payload.ModifiedBy,
                                    Modified = DateTime.Now,
                                    Created = DateTime.Now,
                                    Version =
                                        0, //payload.StatusCode == 3 || payload.StatusCode == 4 ? document.SubmitCount : document.SubmitCount + 1,
                                    Deleted = false,
                                    CreatedBy = payload.ModifiedBy,
                                    ModifiedBy = payload.ModifiedBy,
                                    IsFinal = payload.StatusCode == AppDocumentStatuses.BAN_HANH,
                                    FileType = 0
                                };

                                _documentFileRepository.Add(documentFile);
                                await _documentFileRepository.SaveChanges();
                            }
                        }
                    }

                    //var approvalsId = approvals.Select(x => x.UserId).ToHashSet();
                    //var newUsers = payload.Users?.Where(x => !approvalsId.Contains(x)).ToList() ?? new List<Guid>();
                    if (payload.Users != null && payload.Users.Any())
                    {
                        var reviewItems = new List<TblDocumentReview>();
                        foreach (var userId in payload.Users)
                        {
                            reviewItems.Add(new TblDocumentReview()
                            {
                                DocId = payload.Id,
                                UserId = userId,
                                SubmitCount = document.SubmitCount,
                                DocumentHistoryId = newHistory.Id,
                                Viewed = false,
                                Deleted = false,
                                Modified = DateTime.Now,
                                Created = DateTime.Now,
                                IsAssigned = true,
                                CreatedBy = payload.CreatedBy
                            });
                        }

                        await _documentReviewRepository.AddRangeAsync(reviewItems);
                        await _documentReviewRepository.SaveChanges();
                    }
                    var smsApprovals = await _documentReviewRepository.GetMulti(x => x.DocId == payload.Id && x.IsActiveSMS == true);
                    if (smsApprovals.Any())
                    {
                        _documentReviewRepository.RemoveRange(smsApprovals);
                        await _documentReviewRepository.SaveChanges();
                    }
                    //var smsApprovalsId = smsApprovals.Select(x => x.UserId).ToHashSet();

                    //var newSMSUsers = payload.UsersSMS?.Where(x => !smsApprovalsId.Contains(x)).ToList() ?? new List<Guid>();
                    if (payload.UsersSMS != null && payload.UsersSMS.Any())
                    {
                        var smsReviewItems = new List<TblDocumentReview>();

                        foreach (var userId in payload.UsersSMS)
                        {
                            smsReviewItems.Add(new TblDocumentReview()
                            {
                                DocId = payload.Id,
                                UserId = userId,
                                SubmitCount = null,
                                DocumentHistoryId = newHistory.Id,
                                Viewed = false,
                                Deleted = false,
                                Modified = DateTime.Now,
                                Created = DateTime.Now,
                                IsAssigned = false,     
                                IsActiveSMS = true,
                                CreatedBy = payload.CreatedBy
                            });
                        }

                        await _documentReviewRepository.AddRangeAsync(smsReviewItems);
                        await _documentReviewRepository.SaveChanges();
                    }

                }
                else
                {
                    throw new Exception("Not found");
                }
                return _mapper.Map<DocumentDTO>(document);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private async Task<DocumentHistoryDTO> CreateDocumentHistoryAsync(TblDocument document)
        {
            try
            {
                var isApproved = document.StatusCode == AppDocumentStatuses.PHE_DUYET;
                var historyData = new DocumentHistoryDTO()
                {
                    DocumentId = document.Id,
                    DocumentTitle = document.Title,
                    Note = isApproved ? "Tờ trình đã được phê duyệt"
                    : (document.PreviousStatusCode == AppDocumentStatuses.DU_THAO
                     ? "Tờ trình được gửi lần đầu"
                     : $"Tờ trình được gửi lần {document.SubmitCount}"),
                    Comment = isApproved ? "Đã sửa theo yêu cầu" : "Trình xin ý kiến",
                    Created = DateTime.Now,
                    CreatedBy = document.CreatedBy,
                    DocumentStatus = _statusRepository
                 .FirstOrDefault(s => s.StatusCode == AppDocumentStatuses.XIN_Y_KIEN)?.Id ?? 0,
                };
                return await _historyService.Create(historyData);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                return null;
            }
        }
        public async Task<DocumentDTO?> ReUpdateDocumentAsync(int documentId)
        {
            var currentDocument = await _documentRepository.FirstOrDefaultAsync(x => x.Id == documentId);
            if (currentDocument == null)
            {
                return null;
            }
           
            currentDocument.StatusCode = AppDocumentStatuses.XIN_Y_KIEN_LAI;
            currentDocument.Modified = DateTime.Now;
            currentDocument.PreviousStatusCode = AppDocumentStatuses.PHE_DUYET;
            //currentDocument.SubmitCount += 1;

            _documentRepository.Update(currentDocument);
            await _documentRepository.SaveChanges();

            return _mapper.Map<DocumentDTO>(currentDocument);
        }

        public async Task<bool> SaveFile(IFormFile file, DocumentDTO payload, int fileType = 0)
        {
            try
            {
                var documentData = _mapper.Map<TblDocument>(payload);
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var fileExtension = Path.GetExtension(file.FileName);
                var currentTime = DateTime.Now.ToString("yyyyMMddHHmmssfffttt");
                var fileName = $"{originalFileName}_{currentTime}{fileExtension}";
                var filePathResult = await UploadFile(file);

                var documentFile = new TblDocumentFile
                {
                    Id = 0,
                    FileName = fileName,
                    FilePath = filePathResult.Item1,
                    FilePathToView = filePathResult.Item2,
                    DocId = documentData.Id,
                    UserId = payload.ModifiedBy,
                    Modified = DateTime.Now,
                    Created = DateTime.Now,
                    Version = 1, //documentData.SubmitCount,
                    Deleted = false,
                    CreatedBy = payload.ModifiedBy,
                    ModifiedBy = payload.ModifiedBy,
                    IsFinal = false,
                    FileType = fileType
                };

                await _documentFileRepository.AddAsync(documentFile);
                await _documentFileRepository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<TblDocumentFile> SaveFileSigned(IFormFile file, DocumentDTO payload, int fileType = 0)
        {
            try
            {
                var documentData = _mapper.Map<TblDocument>(payload);
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var fileExtension = Path.GetExtension(file.FileName);
                var currentTime = DateTime.Now.ToString("yyyyMMddHHmmssfffttt");
                var fileName = $"{originalFileName}_{currentTime}{fileExtension}";
                var filePathResult = await UploadFile(file);

                var documentFile = new TblDocumentFile
                {
                    Id = 0,
                    FileName = fileName,
                    FilePath = filePathResult.Item1,
                    FilePathToView = filePathResult.Item2,
                    DocId = documentData.Id,
                    UserId = payload.ModifiedBy,
                    Modified = DateTime.Now,
                    Created = DateTime.Now,
                    Version = 1, //documentData.SubmitCount,
                    Deleted = false,
                    CreatedBy = payload.ModifiedBy,
                    ModifiedBy = payload.ModifiedBy,
                    IsFinal = false,
                    FileType = fileType
                };

                await _documentFileRepository.AddAsync(documentFile);
                await _documentFileRepository.SaveChanges();
                return documentFile;
            }
            catch (Exception ex)
            {
                return new TblDocumentFile();
            }
        }

        public async Task<DocumentFileDTO> GDSignedFile(DocumentFileDTO document)
        {
            var data = _mapper.Map<TblDocumentFile>(document);
            try
            {
                await _documentFileRepository.AddAsync(data);
                await _documentFileRepository.SaveChanges();
                if (document.IsFinal == true)
                {
                    await UpdateDocumentStatus(document.DocId.Value, "cho-ban-hanh");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _mapper.Map<DocumentFileDTO>(data);
        }

        public async Task<DocumentFileDTO> GDSignedFileForce(DocumentFileDTO document)
        {
            var data = _mapper.Map<TblDocumentFile>(document);
            try
            {
                await _documentFileRepository.AddAsync(data);
                await _documentFileRepository.SaveChanges();
                if (document.IsFinal == true)
                {
                    var documentData = await _documentRepository.FirstOrDefaultAsync(x => x.Id == document.DocId);
                    var historyData = new DocumentHistoryDTO()
                    {
                        DocumentId = documentData.Id,
                        DocumentTitle = documentData.Title,
                        Note = @"Tờ trình được ký số trực tiếp và chờ ban hành.",
                        Created = DateTime.Now,
                        CreatedBy = document.UserId,
                        DocumentStatus = _statusRepository
                            .FirstOrDefault(s => s.StatusCode == AppDocumentStatuses.CHO_BAN_HANH).Id,
                    };
                    var historicalData = await _historyService.Create(historyData);
                    await UpdateApprovalByHistiricalId(documentData.Id, historicalData.Id, AppDocumentStatuses.CHO_BAN_HANH);

                    // assign users
                    var reviewItems = new TblDocumentReview()
                    {
                        DocId = documentData.Id,
                        UserId = document.UserId,
                        DocumentHistoryId = historicalData.Id,
                        Comment = @"Tờ trình được ký số trực tiếp và chờ ban hành.",
                        SubmitCount = null,
                        ReviewResult = null,
                        Viewed = true,
                        Deleted = false,
                        Modified = DateTime.Now,
                        Created = DateTime.Now,
                        IsAssigned = true,
                    };

                    await _documentReviewRepository.AddAsync(reviewItems);
                    await _documentReviewRepository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _mapper.Map<DocumentFileDTO>(data);
        }

        public async Task<TblDocumentFile> GetFinalPdf(int docid)
        {
            var docFile = await _documentFileRepository.FirstOrDefaultAsync(x => x.DocId == docid && x.IsFinal == true);
            if (docFile == null)
            {
                // Handle the case where the document is not found
                return null; // Or throw an appropriate exception
            }

            return docFile;
        }

        private async Task RemoveApprovalRecords()
        {
            throw new NotImplementedException();
        }

        private async Task UpdateOverdueStatus()
        {
            throw new NotImplementedException();
        }

        public async Task<List<int>> UpdateApprovalStatus_V2()
        {
            return null;
            //var targetRoleNames = new List<string>() { AppRoleNames.APPROVER, AppRoleNames.GENERAL_APPROVER };
            //var targetRoles = await _roleRepository.GetMulti(x => targetRoleNames.Contains(x.RoleName));
            //var userMapRoles = await _userInRoleRepository.GetMulti(x => targetRoles.Select(x => x.RoleId).Contains(x.RoleId));
            //var users = await _userRepository.GetMulti(x => userMapRoles.Select(x => x.UserId).Contains(x.UserId));
            //var approverCount = users.Count();//6
            //var minimumApproval = Math.Ceiling((double)(approverCount / 2));//3
            //var res = new List<int>();
            //var allPendingDocuments = _documentRepository.Query(x => x.StatusCode == AppDocumentStatuses.PENDING || x.StatusCode == AppDocumentStatuses.OVERDUED).ToList(); //Add rules
            //var allApprovalRecord = await _documentApprovalRepository.Query(x => x.Viewed == false && allPendingDocuments.Select(x => x.Id).Contains((int)x.DocId)).ToListAsync();
            //foreach(var item in allPendingDocuments)
            //{
            //    var allApproval = allApprovalRecord.Where(x => x.DocId == item.Id).ToList();
            //    if(allApproval.Count == approverCount)//6
            //    {
            //        var needAction = 0;
            //        var appCount = allApproval.Where(x => x.StatusCode == AppDocumentStatuses.APPROVERD).ToList().Count;
            //        var commentCount = allApproval.Where(x => x.StatusCode == AppDocumentStatuses.COMMENTED).ToList().Count;
            //        if (appCount + commentCount >= minimumApproval)//3
            //        {
            //            needAction = 1;
            //            res.Add(item.Id);
            //        }
            //        else
            //        {
            //            needAction = -1;
            //        }
            //        if(needAction != 0)
            //        {                        
            //            item.StatusCode = needAction == 1 ? AppDocumentStatuses.APPROVERD : AppDocumentStatuses.DECLINED;   
            //            if(item.StatusCode == AppDocumentStatuses.APPROVERD && commentCount > 0)
            //            {
            //                item.StatusCode = AppDocumentStatuses.COMMENTED;
            //            }
            //            //
            //            if(item.StatusCode == AppDocumentStatuses.DECLINED)
            //            {
            //                item.StatusCode = AppDocumentStatuses.APPROVERD;
            //            }
            //            //
            //            item.IsRetrieved = false;
            //            _documentRepository.Update(item);
            //            await _documentRepository.SaveChanges();
            //            await _notificationService.CreateNotifications(4, item.Id, null);                        
            //        }
            //    }
            //}
            //return res;
        }

        private async Task UpdateApprovalStatus()
        {
            throw new NotImplementedException();
        }

        public async Task<int> PublishDocument(int docId, IFormFile[] files)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RetrieveDocument(DocumentRetrievalRequest payload)
        {
            var currentDocument = await _documentRepository.GetSingleByCondition(d => d.Id == payload.DocumentId) ?? throw new BadHttpRequestException($"Not found DocumentId {payload.DocumentId}");

            // check valid user are able to retrieve document
            var currentReview = await _documentHistoryRepository
                .GetSingleByCondition(dh => dh.DocumentId == currentDocument.Id
                && dh.CreatedBy == payload.CurrentUserId
                && dh.Id == currentDocument.CurrentDocumentHistoricalId);
            if (currentReview == null)
            {
                throw new BadHttpRequestException(@"Người dùng không có quyền thu hồi văn bản.");
            }

            if ((currentDocument.PreviousStatusCode == AppDocumentStatuses.XIN_Y_KIEN)
                && currentDocument.StatusCode != currentDocument.PreviousStatusCode)
            {
                throw new BadHttpRequestException(@"Văn bản đã qua quá trình xin ý kiến nên không thể thu hồi.");
            }
            var retrieveDocument = await _documentReviewRepository.GetAll().Where(d => d.DocId == currentDocument.Id && d.CreatedBy == payload.CurrentUserId && currentDocument.StatusCode != AppDocumentStatuses.XIN_Y_KIEN).OrderByDescending(x => x.Created).FirstOrDefaultAsync();
            if (retrieveDocument != null && retrieveDocument.IsRetrieved == true)
            {
                throw new BadHttpRequestException(@"Tờ trình mới thu hồi không thể thu hồi tiếp.");
            }
            // còn lại các trường hợp: người được assigned đã xem tờ trình hay chưa
            var currentReviews = await _documentReviewRepository
                .GetMulti(dr => dr.DocId == payload.DocumentId
                && dr.DocumentHistoryId == currentDocument.CurrentDocumentHistoricalId
                && dr.UserId != dr.CreatedBy
                && dr.Viewed.HasValue && dr.Viewed.Value);

            if (currentReviews.Any())
            {
                throw new BadHttpRequestException(@"Văn bản đã có người chuyển xử lý xem.");
            }

            var (_, previousStatusId, previousStatusCode) = AppDocumentStatusOrder.GetPreviousOrder(currentDocument.StatusCode, currentDocument.PreviousStatusCode);
            var historyData = new TblDocumentHistory
            {
                DocumentId = payload.DocumentId,
                DocumentTitle = currentDocument.Title,
                Note = payload.Note,
                Comment = payload.Comment,
                Created = DateTime.Now,
                CreatedBy = payload.CurrentUserId,
                DocumentStatus = previousStatusId,
            };
            await _documentHistoryRepository.AddAsync(historyData);
            var newHistory = await _documentHistoryRepository.SaveChangesAsync(historyData);

            // update current reivews
            var currentReviewsByDocId = await _documentReviewRepository
                .GetMulti(r => r.DocId == currentDocument.Id && r.DocumentHistoryId == currentDocument.CurrentDocumentHistoricalId);
            foreach (var review in currentReviewsByDocId)
            {
                review.IsAssigned = false;
            }
            _documentReviewRepository.UpdateRange(currentReviewsByDocId);

            var newReview = new TblDocumentReview
            {
                DocId = currentDocument.Id,
                UserId = payload.CurrentUserId,
                DocumentHistoryId = newHistory.Id,
                IsAssigned = currentDocument.StatusCode != AppDocumentStatuses.XIN_Y_KIEN,
                Created = DateTime.Now,
                CreatedBy = payload.CurrentUserId,
                IsRetrieved = true
            };
            await _documentReviewRepository.AddAsync(newReview);
            await _documentReviewRepository.SaveChanges();

            currentDocument.CurrentDocumentHistoricalId = newHistory.Id;
            currentDocument.PreviousStatusCode = currentDocument.StatusCode;
            currentDocument.StatusCode = previousStatusCode;
            _documentRepository.Update(currentDocument);
            await _documentRepository.SaveChanges();

            return 1;
        }

        public async Task<int> ReturnDocument(DocumentRetrievalRequest payload)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateApprovalByHistiricalId(int documentId, int currentDocumentHistoryId, string statusCode)
        {
            var currentDocument = await _documentRepository.FirstOrDefaultAsync(x => x.Id == documentId) ??
                                  throw new Exception("Document not found");
            currentDocument.CurrentDocumentHistoricalId = currentDocumentHistoryId;
            currentDocument.PreviousStatusCode = currentDocument.StatusCode;
            currentDocument.StatusCode = statusCode;
            _documentRepository.Update(currentDocument);
            await _documentRepository.SaveChanges();
        }
        public async Task<bool> ProcessApproving()
        {
            var currentProcessingTime = DateTime.Now;
            var documentsToApprove = await _documentRepository.GetMulti(x =>
                x.DateEndApproval <= currentProcessingTime &&
                x.StatusCode == AppDocumentStatuses.XIN_Y_KIEN);

            if (!documentsToApprove.Any())
            {
                _logger.LogInformation("No documents found for auto-approval.");
                return false;
            }

            foreach (var document in documentsToApprove)
            {
                var pendingReviews = await _documentReviewRepository.GetMulti(r =>
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

                _documentReviewRepository.UpdateRange(pendingReviews);
                await _documentReviewRepository.SaveChanges();

                var allReviews = await _documentReviewRepository.FindAsync(dr => dr.DocId == document.Id);
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
                        Created = document.DateEndApproval ?? DateTime.Now,
                    };
                    await _documentHistoryRepository.AddAsync(history);
                    await _documentHistoryRepository.SaveChanges();

                    document.StatusCode = AppDocumentStatuses.PHE_DUYET;
                    document.Modified = DateTime.Now;
                    document.CurrentDocumentHistoricalId = history.Id;
                    document.PreviousStatusCode = AppDocumentStatuses.XIN_Y_KIEN;

                    _documentRepository.Update(document);
                    await _documentRepository.SaveChanges();

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
                    await _documentReviewRepository.AddAsync(documentReview);
                    await _documentReviewRepository.SaveChanges();

                    _logger.LogInformation($"Document {document.Id} status updated to 'phe-duyet'");
                }
            }

            return true;
        }

        private async Task<List<Guid>> GetUserIdsByDocumentReviewAsync(int docId)
        {
            return new List<Guid>();
        }
    }
}