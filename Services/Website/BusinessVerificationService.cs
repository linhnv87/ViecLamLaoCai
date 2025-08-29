using AutoMapper;
using Database.Models;
using Database.Models.Website;
using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public interface IBusinessVerificationService
    {
        Task<BusinessVerificationResponseDTO> SubmitVerificationAsync(BusinessVerificationRequestDTO request);
        Task<BusinessVerificationResponseDTO> GetVerificationStatusAsync(int companyId);
        Task<BusinessVerificationResponseDTO> GetVerificationStatusByUserIdAsync(Guid userId);
        Task<List<BusinessVerificationResponseDTO>> GetVerificationHistoryAsync(int companyId);
        Task<bool> IsCompanyVerifiedAsync(int companyId);
        Task<bool> IsCompanyVerifiedByUserIdAsync(Guid userId);
        Task<string> ResendVerificationEmailAsync(int verificationId);
        Task<string> CancelVerificationAsync(int verificationId);
        Task<CompanyInfoDTO> GetCompanyInfoByUserIdAsync(Guid userId);
    }

    public class BusinessVerificationService : IBusinessVerificationService
    {
        private readonly ILogger<BusinessVerificationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _uploadPath;
        private readonly QLTTrContext _context;

        public BusinessVerificationService(
            ILogger<BusinessVerificationService> logger, 
            IConfiguration configuration,
            QLTTrContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _uploadPath = _configuration["FileUpload:Path"] ?? "wwwroot/uploads/verification";
        }

        public async Task<BusinessVerificationResponseDTO> SubmitVerificationAsync(BusinessVerificationRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Business verification submission started for company: {CompanyName}", request.CompanyName);

                var validationResult = ValidateVerificationRequest(request);
                if (!validationResult.IsValid)
                {
                    throw new Exception("Validation failed: " + string.Join(", ", validationResult.Errors));
                }

                var fileValidationResult = await ValidateAndSaveFilesAsync(request.Documents);
                if (!fileValidationResult.IsValid)
                {
                    throw new Exception("File upload failed: " + string.Join(", ", fileValidationResult.Errors));
                }

                var company = await _context.Companies
                    .Where(c => c.UserId == request.UserId)
                    .FirstOrDefaultAsync();
                
                if (company == null)
                {
                    throw new Exception("Không tìm thấy thông tin doanh nghiệp cho user này");
                }
                
                var companyId = company.CompanyId;
                request.CompanyId = companyId;
                await UpdateCompanyInfoAsync(companyId, request);

                var verificationCode = GenerateVerificationCode();

                var approvalRecord = new BusinessApproval
                {
                    CompanyId = companyId,
                    ApprovalStatus = "pending",
                    SubmittedDate = DateTime.UtcNow,
                    VerificationCode = verificationCode,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                };

                _context.BusinessApprovals.Add(approvalRecord);
                await _context.SaveChangesAsync();

                await SaveVerificationDocumentsAsync(approvalRecord.ApprovalId, request.Documents);

                // TODO: Send confirmation email
                // await SendVerificationEmailAsync(request.Email, verificationCode);

                _logger.LogInformation("Business verification submitted successfully. Code: {VerificationCode}", verificationCode);

                return new BusinessVerificationResponseDTO
                {
                    Id = approvalRecord.ApprovalId,
                    CompanyId = approvalRecord.CompanyId,
                    Status = approvalRecord.ApprovalStatus,
                    SubmittedDate = approvalRecord.SubmittedDate,
                    VerificationCode = approvalRecord.VerificationCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting business verification");
                throw new Exception("Lỗi xảy ra khi gửi yêu cầu xác thực: " + ex.Message);
            }
        }

        public async Task<BusinessVerificationResponseDTO> GetVerificationStatusAsync(int companyId)
        {
            try
            {
                _logger.LogInformation("Getting verification status for company: {CompanyId}", companyId);

                var approval = await _context.BusinessApprovals
                    .Where(a => a.CompanyId == companyId)
                    .OrderByDescending(a => a.SubmittedDate)
                    .FirstOrDefaultAsync();

                if (approval == null)
                {
                    throw new Exception("Không tìm thấy yêu cầu xác thực cho doanh nghiệp này");
                }

                return new BusinessVerificationResponseDTO
                {
                    Id = approval.ApprovalId,
                    CompanyId = approval.CompanyId,
                    Status = approval.ApprovalStatus,
                    SubmittedDate = approval.SubmittedDate,
                    ReviewedDate = approval.ApprovalDate,
                    ReviewerNotes = approval.Notes,
                    VerificationCode = approval.VerificationCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting verification status for company: {CompanyId}", companyId);
                throw new Exception("Lỗi xảy ra khi lấy trạng thái xác thực: " + ex.Message);
            }
        }

        public async Task<BusinessVerificationResponseDTO> GetVerificationStatusByUserIdAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting verification status for user: {UserId}", userId);

                var company = await _context.Companies
                    .Where(c => c.UserId == userId)
                    .FirstOrDefaultAsync();

                if (company == null)
                {
                    _logger.LogWarning("No company found for user: {UserId}, returning empty verification status", userId);
                    return new BusinessVerificationResponseDTO
                    {
                        Id = 0,
                        CompanyId = 0,
                        Status = "not_verified",
                        SubmittedDate = DateTime.MinValue,
                        ReviewedDate = DateTime.MinValue,
                        ReviewerNotes = "",
                        VerificationCode = ""
                    };
                }

                var approval = await _context.BusinessApprovals
                    .Where(a => a.CompanyId == company.CompanyId)
                    .OrderByDescending(a => a.SubmittedDate)
                    .FirstOrDefaultAsync();

                if (approval == null)
                {
                    _logger.LogWarning("No verification request found for company: {CompanyId}", company.CompanyId);
                    return new BusinessVerificationResponseDTO
                    {
                        Id = 0,
                        CompanyId = company.CompanyId,
                        Status = "not_verified",
                        SubmittedDate = DateTime.MinValue,
                        ReviewedDate = DateTime.MinValue,
                        ReviewerNotes = "",
                        VerificationCode = ""
                    };
                }

                return new BusinessVerificationResponseDTO
                {
                    Id = approval.ApprovalId,
                    CompanyId = approval.CompanyId,
                    Status = approval.ApprovalStatus,
                    SubmittedDate = approval.SubmittedDate,
                    ReviewedDate = approval.ApprovalDate,
                    ReviewerNotes = approval.Notes,
                    VerificationCode = approval.VerificationCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting verification status for user: {UserId}", userId);
                throw new Exception("Lỗi xảy ra khi lấy trạng thái xác thực cho user: " + ex.Message);
            }
        }

        public async Task<List<BusinessVerificationResponseDTO>> GetVerificationHistoryAsync(int companyId)
        {
            try
            {
                _logger.LogInformation("Getting verification history for company: {CompanyId}", companyId);

                var history = await _context.BusinessApprovals
                    .Where(a => a.CompanyId == companyId)
                    .OrderByDescending(a => a.SubmittedDate)
                    .ToListAsync();

                if (!history.Any())
                {
                    throw new Exception("Không tìm thấy lịch sử xác thực cho doanh nghiệp này");
                }

                return history.Select(a => new BusinessVerificationResponseDTO
                {
                    Id = a.ApprovalId,
                    CompanyId = a.CompanyId,
                    Status = a.ApprovalStatus,
                    SubmittedDate = a.SubmittedDate,
                    ReviewedDate = a.ApprovalDate,
                    ReviewerNotes = a.Notes,
                    VerificationCode = a.VerificationCode
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting verification history for company: {CompanyId}", companyId);
                throw new Exception("Lỗi xảy ra khi lấy lịch sử xác thực: " + ex.Message);
            }
        }

        public async Task<bool> IsCompanyVerifiedAsync(int companyId)
        {
            try
            {
                _logger.LogInformation("Checking verification status for company: {CompanyId}", companyId);

                var approval = await _context.BusinessApprovals
                    .Where(a => a.CompanyId == companyId && a.ApprovalStatus == "approved")
                    .OrderByDescending(a => a.SubmittedDate)
                    .FirstOrDefaultAsync();

                return approval != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking verification status for company: {CompanyId}", companyId);
                throw new Exception("Lỗi xảy ra khi kiểm tra trạng thái xác thực: " + ex.Message);
            }
        }

        public async Task<bool> IsCompanyVerifiedByUserIdAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Checking verification status for user: {UserId}", userId);

                var company = await _context.Companies
                    .Where(c => c.UserId == userId)
                    .FirstOrDefaultAsync();

                if (company == null)
                {
                    _logger.LogWarning("No company found for user: {UserId}", userId);
                    return false;
                }

                var approval = await _context.BusinessApprovals
                    .Where(a => a.CompanyId == company.CompanyId && a.ApprovalStatus == "approved")
                    .OrderByDescending(a => a.SubmittedDate)
                    .FirstOrDefaultAsync();

                return approval != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking verification status for user: {UserId}", userId);
                throw new Exception("Lỗi xảy ra khi kiểm tra trạng thái xác thực: " + ex.Message);
            }
        }

        public async Task<string> ResendVerificationEmailAsync(int verificationId)
        {
            try
            {
                _logger.LogInformation("Resending verification email for verification: {VerificationId}", verificationId);

                var approval = await _context.BusinessApprovals
                    .Include(a => a.Company)
                    .FirstOrDefaultAsync(a => a.ApprovalId == verificationId);

                if (approval == null)
                {
                    throw new Exception("Không tìm thấy yêu cầu xác thực");
                }

                // TODO: Send verification email
                // await SendVerificationEmailAsync(approval.Company.Email, approval.VerificationCode);

                var message = $"Email xác thực đã được gửi lại đến {approval.Company.Email}";

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending verification email for verification: {VerificationId}", verificationId);
                throw new Exception("Lỗi xảy ra khi gửi lại email xác thực: " + ex.Message);
            }
        }

        public async Task<string> CancelVerificationAsync(int verificationId)
        {
            try
            {
                _logger.LogInformation("Cancelling verification: {VerificationId}", verificationId);

                var approval = await _context.BusinessApprovals
                    .FirstOrDefaultAsync(a => a.ApprovalId == verificationId);

                if (approval == null)
                {
                    throw new Exception("Không tìm thấy yêu cầu xác thực");
                }

                if (approval.ApprovalStatus != "pending")
                {
                    throw new Exception("Chỉ có thể hủy yêu cầu xác thực đang chờ xử lý");
                }

                approval.ApprovalStatus = "cancelled";
                approval.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var message = $"Yêu cầu xác thực {approval.VerificationCode} đã được hủy";

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling verification: {VerificationId}", verificationId);
                throw new Exception("Lỗi xảy ra khi hủy yêu cầu xác thực: " + ex.Message);
            }
        }

        public async Task<CompanyInfoDTO> GetCompanyInfoByUserIdAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting company info for user: {UserId}", userId);

                var company = await _context.Companies
                    .Include(c => c.AppUser)
                    .Where(c => c.UserId == userId)
                    .FirstOrDefaultAsync();

                if (company == null)
                {
                    _logger.LogWarning("No company found for user: {UserId}, returning empty company info", userId);
                    return new CompanyInfoDTO
                    {
                        CompanyId = 0,
                        CompanyName = "",
                        Email = "",
                        PhoneNumber = "",
                        Address = "",
                        RepresentativeName = "",
                        TaxNumber = "",
                        Website = "",
                        CompanySize = "",
                        Industry = "",
                        Description = "",
                        Position = "",
                        IsVerified = false
                    };
                }

                var isVerified = await _context.BusinessApprovals
                    .Where(a => a.CompanyId == company.CompanyId && a.ApprovalStatus == "approved")
                    .AnyAsync();

                return new CompanyInfoDTO
                {
                    CompanyId = company.CompanyId,
                    CompanyName = company.CompanyName ?? "",
                    Email = company.Email ?? "",
                    PhoneNumber = company.PhoneNumber ?? "",
                    Address = company.Address ?? "",
                    RepresentativeName = company.AppUser?.UserFullName ?? "",
                    TaxNumber = company.TaxNumber ?? "",
                    Website = company.Website ?? "",
                    CompanySize = company.CompanySize ?? "",
                    Industry = company.Industry ?? "",
                    Description = company.Description ?? "",
                    Position = company.Position ?? "",
                    IsVerified = isVerified
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company info for user: {UserId}", userId);
                throw new Exception("Lỗi xảy ra khi lấy thông tin doanh nghiệp: " + ex.Message);
            }
        }

        #region Private Methods

        private async Task UpdateCompanyInfoAsync(int companyId, BusinessVerificationRequestDTO request)
        {
            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
            {
                throw new Exception("Không tìm thấy thông tin doanh nghiệp");
            }

            if (!string.IsNullOrWhiteSpace(request.CompanyName) && company.CompanyName != request.CompanyName)
                company.CompanyName = request.CompanyName;

            if (!string.IsNullOrWhiteSpace(request.Email) && company.Email != request.Email)
                company.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.Phone) && company.PhoneNumber != request.Phone)
                company.PhoneNumber = request.Phone;

            if (!string.IsNullOrWhiteSpace(request.Address) && company.Address != request.Address)
                company.Address = request.Address;

            if (!string.IsNullOrWhiteSpace(request.Website) && company.Website != request.Website)
                company.Website = request.Website;

            if (!string.IsNullOrWhiteSpace(request.CompanySize) && company.CompanySize != request.CompanySize)
                company.CompanySize = request.CompanySize;

            if (!string.IsNullOrWhiteSpace(request.Description) && company.Description != request.Description)
                company.Description = request.Description;

            if (!string.IsNullOrWhiteSpace(request.TaxNumber) && company.TaxNumber != request.TaxNumber)
                company.TaxNumber = request.TaxNumber;

            if (!string.IsNullOrWhiteSpace(request.Position) && company.Position != request.Position)
                company.Position = request.Position;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated company info for CompanyId: {CompanyId}", companyId);
        }

        private async Task SaveVerificationDocumentsAsync(int approvalId, IFormFileCollection documents)
        {
            foreach (var file in documents)
            {
                if (file == null || file.Length == 0) continue;

                var filePath = await SaveFileAsync(file);
                var documentType = file.Name ?? "unknown";

                var verificationDocument = new VerificationDocument
                {
                    ApprovalId = approvalId,
                    DocumentType = documentType,
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileSize = file.Length,
                    FileType = Path.GetExtension(file.FileName).ToLowerInvariant(),
                    UploadDate = DateTime.UtcNow
                };

                _context.VerificationDocuments.Add(verificationDocument);
            }

            await _context.SaveChangesAsync();
        }

        private ValidationResult ValidateVerificationRequest(BusinessVerificationRequestDTO request)
        {
            var errors = new List<string>();

            if (request.UserId == Guid.Empty)
                errors.Add("UserId không hợp lệ");

            if (string.IsNullOrWhiteSpace(request.CompanyName))
                errors.Add("Company name is required");
            else if (request.CompanyName.Length < 2)
                errors.Add("Company name must be at least 2 characters");

            if (string.IsNullOrWhiteSpace(request.TaxNumber))
                errors.Add("Tax number is required");
            else if (!Regex.IsMatch(request.TaxNumber, @"^\d{10,13}$"))
                errors.Add("Tax number must be 10-13 digits");

            if (string.IsNullOrWhiteSpace(request.Position))
                errors.Add("Position is required");

            if (string.IsNullOrWhiteSpace(request.Phone))
                errors.Add("Phone is required");
            else if (!Regex.IsMatch(request.Phone, @"^[0-9]{10,11}$"))
                errors.Add("Phone must be 10-11 digits");

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required");
            else if (!IsValidEmail(request.Email))
                errors.Add("Invalid email format");

            if (string.IsNullOrWhiteSpace(request.Address))
                errors.Add("Address is required");
            else if (request.Address.Length < 10)
                errors.Add("Address must be at least 10 characters");

            if (!string.IsNullOrWhiteSpace(request.Website) && !request.Website.StartsWith("http://") && !request.Website.StartsWith("https://"))
                errors.Add("Website must start with http:// or https://");

            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }

        private async Task<ValidationResult> ValidateAndSaveFilesAsync(IFormFileCollection documents)
        {
            var errors = new List<string>();
            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

            _logger.LogInformation("Validating {DocumentCount} documents", documents.Count);

            foreach (var file in documents)
            {
                if (file != null)
                {
                    _logger.LogInformation("Received file: Name={FileName}, Size={FileSize}, ContentType={ContentType}", 
                        file.FileName, file.Length, file.ContentType);
                }
            }
            var documentCount = documents.Count;
            var hasRequiredDocuments = documentCount >= 2;

            _logger.LogInformation("Document check: TotalFiles={DocumentCount}, HasRequiredDocuments={HasRequired}", 
                documentCount, hasRequiredDocuments);

            if (!hasRequiredDocuments)
                errors.Add($"Required documents missing. Need at least 2 files (Business License + Representative ID), got {documentCount}");
            foreach (var file in documents)
            {
                if (file != null)
                {
                    _logger.LogInformation("File received: Name={FileName}, Size={FileSize}, ContentType={ContentType}", 
                        file.FileName, file.Length, file.ContentType);
                }
            }

            foreach (var file in documents)
            {
                if (file == null || file.Length == 0) 
                {
                    _logger.LogWarning("Null or empty file received");
                    continue;
                }
                
                _logger.LogInformation("Validating file: {FileName}, Size={FileSize}, Extension={Extension}", 
                    file.FileName, file.Length, Path.GetExtension(file.FileName));

                if (file.Length > maxFileSize)
                {
                    var error = $"File {file.FileName} exceeds 5MB limit";
                    errors.Add(error);
                    _logger.LogWarning(error);
                    continue;
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    var error = $"File {file.FileName} has invalid format. Only PDF, JPG, PNG allowed";
                    errors.Add(error);
                    _logger.LogWarning(error);
                    continue;
                }

                _logger.LogInformation("File {FileName} validation passed", file.FileName);
            }

            _logger.LogInformation("File validation completed. Errors: {ErrorCount}", errors.Count);
            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            Directory.CreateDirectory(_uploadPath);
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("File saved: {FilePath}", filePath);
            return filePath;
        }

        private string GenerateVerificationCode()
        {
            var year = DateTime.Now.Year;
            var random = new Random();
            var sequence = random.Next(1, 1000).ToString("D3");
            return $"VER-{year}-{sequence}";
        }

        private int GenerateId()
        {
            return new Random().Next(1000, 9999);
        }





        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
