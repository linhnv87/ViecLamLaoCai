using AutoMapper;
using Database.Models;
using Database.Models.Website;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class BusinessApprovalService : IBusinessApprovalService
    {
        private readonly ILogger<BusinessApprovalService> _logger;
        private readonly IConfiguration _configuration;
        private readonly QLTTrContext _context;
        private readonly IMapper _mapper;
        private readonly string _uploadPath;

        public BusinessApprovalService(
            ILogger<BusinessApprovalService> logger,
            IConfiguration configuration,
            QLTTrContext context,
            IMapper mapper)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
            _uploadPath = _configuration["FileUpload:Path"] ?? "wwwroot/uploads/verification";
        }

        public async Task<PaginatedBusinessApprovalResponseDTO> GetBusinessApprovalsAsync(BusinessApprovalFilterDTO filter)
        {
            try
            {
                _logger.LogInformation("Getting business approvals with filter: {@Filter}", filter);

                var query = _context.BusinessApprovals
                    .Include(ba => ba.Company)
                    .Include(ba => ba.VerificationDocuments)
                    .Where(ba => ba.IsActive);

                // Apply status filter
                if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "all")
                {
                    query = query.Where(ba => ba.ApprovalStatus == filter.Status);
                }

                // Apply business type filter
                if (!string.IsNullOrEmpty(filter.BusinessType) && filter.BusinessType != "all")
                {
                    query = query.Where(ba => ba.Company.Industry == filter.BusinessType);
                }

                // Apply keyword search
                if (!string.IsNullOrEmpty(filter.SearchKeyword))
                {
                    var keyword = filter.SearchKeyword.ToLower();
                    query = query.Where(ba => 
                        ba.Company.CompanyName.ToLower().Contains(keyword) ||
                        ba.Company.TaxNumber.Contains(keyword) ||
                        ba.Company.Email.ToLower().Contains(keyword));
                }

                // Apply sorting
                switch (filter.SortBy?.ToLower())
                {
                    case "businessname":
                        query = filter.SortOrder?.ToLower() == "asc" 
                            ? query.OrderBy(ba => ba.Company.CompanyName)
                            : query.OrderByDescending(ba => ba.Company.CompanyName);
                        break;
                    case "status":
                        query = filter.SortOrder?.ToLower() == "asc"
                            ? query.OrderBy(ba => ba.ApprovalStatus)
                            : query.OrderByDescending(ba => ba.ApprovalStatus);
                        break;
                    default:
                        query = filter.SortOrder?.ToLower() == "asc"
                            ? query.OrderBy(ba => ba.SubmittedDate)
                            : query.OrderByDescending(ba => ba.SubmittedDate);
                        break;
                }

                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalItems / filter.PageSize);

                var businesses = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(ba => new BusinessApprovalListResponseDTO
                    {
                        Id = ba.ApprovalId,
                        BusinessName = ba.Company.CompanyName,
                        TaxCode = ba.Company.TaxNumber,
                        ContactPerson = ba.Company.AppUser.UserFullName,
                        Email = ba.Company.Email,
                        Phone = ba.Company.PhoneNumber,
                        Address = ba.Company.Address,
                        BusinessType = ba.Company.Industry,
                        RegistrationDate = ba.SubmittedDate,
                        Status = ba.ApprovalStatus,
                        LogoUrl = ba.Company.LogoUrl ?? "assets/vieclamlaocai/img/image 16.png",
                        Notes = ba.Notes,
                        ApprovalDate = ba.ApprovalDate,
                        ApprovedBy = ba.ApprovedBy,
                        CreatedDate = ba.CreatedDate,
                        ModifiedDate = ba.ModifiedDate,
                        Documents = ba.VerificationDocuments.Select(vd => new BusinessDocumentDTO
                        {
                            Id = vd.Id,
                            Name = vd.FileName,
                            Type = vd.FileType,
                            UploadDate = vd.UploadDate,
                            Verified = true, // We'll implement document verification later
                            FileSize = vd.FileSize,
                            FilePath = vd.FilePath
                        }).ToList()
                    })
                    .ToListAsync();

                return new PaginatedBusinessApprovalResponseDTO
                {
                    Data = businesses,
                    CurrentPage = filter.Page,
                    PageSize = filter.PageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasPreviousPage = filter.Page > 1,
                    HasNextPage = filter.Page < totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business approvals");
                throw new Exception("Lỗi xảy ra khi lấy danh sách doanh nghiệp: " + ex.Message);
            }
        }

        public async Task<BusinessApprovalStatsDTO> GetApprovalStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Getting business approval statistics");

                var today = DateTime.Today;
                var stats = new BusinessApprovalStatsDTO();

                var businessApprovals = await _context.BusinessApprovals
                    .Where(ba => ba.IsActive)
                    .ToListAsync();

                stats.Pending = businessApprovals.Count(ba => ba.ApprovalStatus == "pending");
                stats.Approved = businessApprovals.Count(ba => ba.ApprovalStatus == "approved");
                stats.Rejected = businessApprovals.Count(ba => ba.ApprovalStatus == "rejected");
                stats.Reviewing = businessApprovals.Count(ba => ba.ApprovalStatus == "reviewing");
                stats.TodaySubmissions = businessApprovals.Count(ba => ba.SubmittedDate.Date == today);
                stats.TotalSubmissions = businessApprovals.Count;

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approval statistics");
                throw new Exception("Lỗi xảy ra khi lấy thống kê: " + ex.Message);
            }
        }

        public async Task<BusinessApprovalDetailResponseDTO> GetBusinessApprovalByIdAsync(int businessId)
        {
            try
            {
                _logger.LogInformation("Getting business approval detail for ID: {BusinessId}", businessId);

                var business = await _context.BusinessApprovals
                    .Include(ba => ba.Company)
                    .ThenInclude(c => c.AppUser)
                    .Include(ba => ba.VerificationDocuments)
                    .Where(ba => ba.ApprovalId == businessId && ba.IsActive)
                    .FirstOrDefaultAsync();

                if (business == null)
                {
                    throw new Exception("Không tìm thấy thông tin doanh nghiệp");
                }

                return new BusinessApprovalDetailResponseDTO
                {
                    Id = business.ApprovalId,
                    BusinessName = business.Company.CompanyName,
                    TaxCode = business.Company.TaxNumber,
                    ContactPerson = business.Company.AppUser?.UserFullName,
                    Email = business.Company.Email,
                    Phone = business.Company.PhoneNumber,
                    Address = business.Company.Address,
                    BusinessType = business.Company.Industry,
                    RegistrationDate = business.SubmittedDate,
                    Status = business.ApprovalStatus,
                    LogoUrl = business.Company.LogoUrl ?? "assets/vieclamlaocai/img/image 16.png",
                    Notes = business.Notes,
                    ApprovalDate = business.ApprovalDate,
                    ApprovedBy = business.ApprovedBy,
                    CreatedDate = business.CreatedDate,
                    ModifiedDate = business.ModifiedDate,
                    Website = business.Company.Website,
                    CompanySize = business.Company.CompanySize,
                    Industry = business.Company.Industry,
                    Description = business.Company.Description,
                    Position = business.Company.Position,
                    Documents = business.VerificationDocuments.Select(vd => new BusinessDocumentDTO
                    {
                        Id = vd.Id,
                        Name = vd.FileName,
                        Type = vd.FileType,
                        UploadDate = vd.UploadDate,
                        Verified = true,
                        FileSize = vd.FileSize,
                        FilePath = vd.FilePath
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business approval detail for ID: {BusinessId}", businessId);
                throw new Exception("Lỗi xảy ra khi lấy chi tiết doanh nghiệp: " + ex.Message);
            }
        }

        public async Task<bool> ApproveBusinessAsync(ApproveBusinessRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Approving business: {@Request}", request);

                var business = await _context.BusinessApprovals
                    .FirstOrDefaultAsync(ba => ba.ApprovalId == request.BusinessId && ba.IsActive);

                if (business == null)
                {
                    throw new Exception("Không tìm thấy thông tin doanh nghiệp");
                }

                if (business.ApprovalStatus == "approved")
                {
                    throw new Exception("Doanh nghiệp đã được phê duyệt trước đó");
                }

                business.ApprovalStatus = "approved";
                business.ApprovedBy = request.ApprovedBy;
                business.ApprovalDate = DateTime.UtcNow;
                business.Notes = request.Notes;
                business.ModifiedDate = DateTime.UtcNow;

                // Update company verification status
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == business.CompanyId);
                if (company != null)
                {
                    company.IsVerified = true;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Business approved successfully: {BusinessId}", request.BusinessId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving business: {BusinessId}", request.BusinessId);
                throw new Exception("Lỗi xảy ra khi phê duyệt doanh nghiệp: " + ex.Message);
            }
        }

        public async Task<bool> RejectBusinessAsync(RejectBusinessRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Rejecting business: {@Request}", request);

                var business = await _context.BusinessApprovals
                    .FirstOrDefaultAsync(ba => ba.ApprovalId == request.BusinessId && ba.IsActive);

                if (business == null)
                {
                    throw new Exception("Không tìm thấy thông tin doanh nghiệp");
                }

                if (business.ApprovalStatus == "approved")
                {
                    throw new Exception("Không thể từ chối doanh nghiệp đã được phê duyệt");
                }

                business.ApprovalStatus = "rejected";
                business.ApprovedBy = request.RejectedBy;
                business.ApprovalDate = DateTime.UtcNow;
                business.Notes = request.Reason;
                business.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Business rejected successfully: {BusinessId}", request.BusinessId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting business: {BusinessId}", request.BusinessId);
                throw new Exception("Lỗi xảy ra khi từ chối doanh nghiệp: " + ex.Message);
            }
        }

        public async Task<bool> SetReviewingBusinessAsync(SetReviewingRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Setting business to reviewing: {@Request}", request);

                var business = await _context.BusinessApprovals
                    .FirstOrDefaultAsync(ba => ba.ApprovalId == request.BusinessId && ba.IsActive);

                if (business == null)
                {
                    throw new Exception("Không tìm thấy thông tin doanh nghiệp");
                }

                if (business.ApprovalStatus != "pending")
                {
                    throw new Exception("Chỉ có thể chuyển trạng thái đang xem xét cho doanh nghiệp đang chờ duyệt");
                }

                business.ApprovalStatus = "reviewing";
                business.ApprovedBy = request.ReviewedBy;
                business.Notes = request.Notes;
                business.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Business set to reviewing successfully: {BusinessId}", request.BusinessId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting business to reviewing: {BusinessId}", request.BusinessId);
                throw new Exception("Lỗi xảy ra khi chuyển trạng thái xem xét: " + ex.Message);
            }
        }

        public async Task<List<BusinessDocumentDTO>> GetBusinessDocumentsAsync(int businessId)
        {
            try
            {
                _logger.LogInformation("Getting documents for business: {BusinessId}", businessId);

                var documents = await _context.VerificationDocuments
                    .Where(vd => vd.ApprovalId == businessId)
                    .Select(vd => new BusinessDocumentDTO
                    {
                        Id = vd.Id,
                        Name = vd.FileName,
                        Type = vd.FileType,
                        UploadDate = vd.UploadDate,
                        Verified = true,
                        FileSize = vd.FileSize,
                        FilePath = vd.FilePath
                    })
                    .ToListAsync();

                return documents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting documents for business: {BusinessId}", businessId);
                throw new Exception("Lỗi xảy ra khi lấy danh sách tài liệu: " + ex.Message);
            }
        }

        public async Task<(byte[] fileContent, string fileName, string contentType)> DownloadDocumentAsync(int documentId)
        {
            try
            {
                _logger.LogInformation("Downloading document: {DocumentId}", documentId);

                var document = await _context.VerificationDocuments
                    .FirstOrDefaultAsync(vd => vd.Id == documentId);

                if (document == null)
                {
                    throw new Exception("Không tìm thấy tài liệu");
                }

                if (!File.Exists(document.FilePath))
                {
                    throw new Exception("Tệp tin không tồn tại trên hệ thống");
                }

                var fileContent = await File.ReadAllBytesAsync(document.FilePath);
                var contentType = GetContentType(document.FileType);

                return (fileContent, document.FileName, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document: {DocumentId}", documentId);
                throw new Exception("Lỗi xảy ra khi tải xuống tài liệu: " + ex.Message);
            }
        }

        public async Task<bool> VerifyDocumentAsync(int documentId, string verifiedBy)
        {
            try
            {
                _logger.LogInformation("Verifying document: {DocumentId} by {VerifiedBy}", documentId, verifiedBy);

                var document = await _context.VerificationDocuments
                    .FirstOrDefaultAsync(vd => vd.Id == documentId);

                if (document == null)
                {
                    throw new Exception("Không tìm thấy tài liệu");
                }

                // Add verification logic here if needed
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying document: {DocumentId}", documentId);
                throw new Exception("Lỗi xảy ra khi xác minh tài liệu: " + ex.Message);
            }
        }

        public async Task<List<BusinessApprovalHistoryDTO>> GetApprovalHistoryAsync(int businessId)
        {
            try
            {
                // This would require a separate ApprovalHistory table
                // For now, return empty list
                return new List<BusinessApprovalHistoryDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approval history: {BusinessId}", businessId);
                throw new Exception("Lỗi xảy ra khi lấy lịch sử duyệt: " + ex.Message);
            }
        }

        public async Task<List<BusinessApprovalListResponseDTO>> GetBusinessesByStatusAsync(string status)
        {
            try
            {
                var businesses = await _context.BusinessApprovals
                    .Include(ba => ba.Company)
                    .Include(ba => ba.VerificationDocuments)
                    .Where(ba => ba.IsActive && ba.ApprovalStatus == status)
                    .Select(ba => new BusinessApprovalListResponseDTO
                    {
                        Id = ba.ApprovalId,
                        BusinessName = ba.Company.CompanyName,
                        TaxCode = ba.Company.TaxNumber,
                        ContactPerson = ba.Company.AppUser.UserFullName,
                        Email = ba.Company.Email,
                        Phone = ba.Company.PhoneNumber,
                        Address = ba.Company.Address,
                        BusinessType = ba.Company.Industry,
                        RegistrationDate = ba.SubmittedDate,
                        Status = ba.ApprovalStatus,
                        LogoUrl = ba.Company.LogoUrl ?? "assets/vieclamlaocai/img/image 16.png"
                    })
                    .ToListAsync();

                return businesses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting businesses by status: {Status}", status);
                throw new Exception("Lỗi xảy ra khi lấy doanh nghiệp theo trạng thái: " + ex.Message);
            }
        }

        public async Task<List<BusinessApprovalListResponseDTO>> SearchBusinessesAsync(string keyword)
        {
            try
            {
                var lowerKeyword = keyword.ToLower();
                var businesses = await _context.BusinessApprovals
                    .Include(ba => ba.Company)
                    .Where(ba => ba.IsActive && 
                        (ba.Company.CompanyName.ToLower().Contains(lowerKeyword) ||
                         ba.Company.TaxNumber.Contains(keyword) ||
                         ba.Company.Email.ToLower().Contains(lowerKeyword)))
                    .Select(ba => new BusinessApprovalListResponseDTO
                    {
                        Id = ba.ApprovalId,
                        BusinessName = ba.Company.CompanyName,
                        TaxCode = ba.Company.TaxNumber,
                        ContactPerson = ba.Company.AppUser.UserFullName,
                        Email = ba.Company.Email,
                        Phone = ba.Company.PhoneNumber,
                        Address = ba.Company.Address,
                        BusinessType = ba.Company.Industry,
                        RegistrationDate = ba.SubmittedDate,
                        Status = ba.ApprovalStatus,
                        LogoUrl = ba.Company.LogoUrl ?? "assets/vieclamlaocai/img/image 16.png"
                    })
                    .ToListAsync();

                return businesses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching businesses: {Keyword}", keyword);
                throw new Exception("Lỗi xảy ra khi tìm kiếm doanh nghiệp: " + ex.Message);
            }
        }

        public async Task<int> GetTodaySubmissionsCountAsync()
        {
            try
            {
                var today = DateTime.Today;
                var count = await _context.BusinessApprovals
                    .Where(ba => ba.IsActive && ba.SubmittedDate.Date == today)
                    .CountAsync();

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting today submissions count");
                throw new Exception("Lỗi xảy ra khi lấy số lượng đăng ký hôm nay: " + ex.Message);
            }
        }

        public async Task<bool> BulkApproveBusinessesAsync(List<int> businessIds, string approvedBy)
        {
            try
            {
                var businesses = await _context.BusinessApprovals
                    .Where(ba => businessIds.Contains(ba.ApprovalId) && ba.IsActive)
                    .ToListAsync();

                foreach (var business in businesses)
                {
                    business.ApprovalStatus = "approved";
                    business.ApprovedBy = approvedBy;
                    business.ApprovalDate = DateTime.UtcNow;
                    business.ModifiedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk approving businesses");
                throw new Exception("Lỗi xảy ra khi phê duyệt hàng loạt: " + ex.Message);
            }
        }

        public async Task<bool> BulkRejectBusinessesAsync(List<int> businessIds, string rejectedBy, string reason)
        {
            try
            {
                var businesses = await _context.BusinessApprovals
                    .Where(ba => businessIds.Contains(ba.ApprovalId) && ba.IsActive)
                    .ToListAsync();

                foreach (var business in businesses)
                {
                    business.ApprovalStatus = "rejected";
                    business.ApprovedBy = rejectedBy;
                    business.ApprovalDate = DateTime.UtcNow;
                    business.Notes = reason;
                    business.ModifiedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk rejecting businesses");
                throw new Exception("Lỗi xảy ra khi từ chối hàng loạt: " + ex.Message);
            }
        }

        public async Task<(byte[] fileContent, string fileName, string contentType)> ExportBusinessApprovalsAsync(BusinessApprovalFilterDTO filter, string format = "excel")
        {
            try
            {
                // This would require implementation of Excel/PDF export
                // For now, return empty content
                return (new byte[0], $"business_approvals_{DateTime.Now:yyyyMMdd}.{format}", "application/octet-stream");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting business approvals");
                throw new Exception("Lỗi xảy ra khi xuất dữ liệu: " + ex.Message);
            }
        }

        #region Private Methods

        private string GetContentType(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }

        #endregion
    }
}
