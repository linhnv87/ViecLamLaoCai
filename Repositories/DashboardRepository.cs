using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database.Models.Website;
using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IDashboardRepository
    {
        // Business Dashboard
        Task<BusinessDashboardDTO> GetBusinessDashboardAsync(int businessId);
        Task<BusinessDashboardDTO> GetBusinessDashboardByUserIdAsync(string userId);
        Task<List<RecentJobDTO>> GetRecentJobsAsync(int businessId, string companyName);
        Task<List<RecentCandidateDTO>> GetRecentCandidatesAsync(int businessId);
        Task<int> GetTotalApplicationsForCompanyAsync(int companyId);
        Task<int> GetTodayApplicationsForCompanyAsync(int companyId, DateTime today);

        // Candidate Dashboard
        Task<CandidateDashboardDTO> GetCandidateDashboardAsync(string candidateId);
        Task<List<SavedJobDTO>> GetSavedJobsAsync(string candidateId);
        Task<List<RecentApplicationDTO>> GetRecentApplicationsAsync(string candidateId);
        Task<List<AppliedJobDTO>> GetAppliedJobsAsync(string candidateId);
        Task<int> GetSuitableJobsCountAsync(string workerId);
        Task<int> GetTotalCVsAsync(string workerId);

        // Admin Dashboard
        Task<AdminDashboardDTO> GetAdminDashboardAsync();
        Task<int> GetTotalBusinessesAsync();
        Task<int> GetPendingApprovalsAsync();
        Task<int> GetApprovedBusinessesAsync();
        Task<int> GetRejectedBusinessesAsync();
        Task<int> GetTotalJobsAsync();
        Task<int> GetTotalCandidatesAsync();
        Task<int> GetTodayRegistrationsAsync(DateTime today);
        Task<int> GetActiveUsersCountAsync();

        // Activities
        Task<List<ActivityDTO>> GetRecentActivitiesAsync(string userId);
    }

    public class DashboardRepository : IDashboardRepository
    {
        private readonly QLTTrContext _context;

        public DashboardRepository(QLTTrContext context)
        {
            _context = context;
        }

        #region Business Dashboard Methods

        public async Task<BusinessDashboardDTO> GetBusinessDashboardAsync(int businessId)
        {
            try
            {
                var company = await _context.Companies
                    .Include(c => c.JobPostings)
                    .FirstOrDefaultAsync(c => c.CompanyId == businessId);

                if (company == null)
                    return null;

            var today = DateTime.Today;
            var totalJobs = company.JobPostings?.Count ?? 0;
            var activeJobs = company.JobPostings?.Count(j => j.IsActive) ?? 0;
            var totalApplications = await GetTotalApplicationsForCompanyAsync(businessId);
            var todayApplications = await GetTodayApplicationsForCompanyAsync(businessId, today);
            var recentJobs = await GetRecentJobsAsync(businessId, company.CompanyName);
            var recentCandidates = await GetRecentCandidatesAsync(businessId);

                return new BusinessDashboardDTO
                {
                    TotalJobs = totalJobs,
                    ActiveJobs = activeJobs,
                    TotalApplications = totalApplications,
                    TotalViews = company.JobPostings?.Sum(jp => jp.Views) ?? 0,
                    TodayApplications = todayApplications,
                    TodayViews = 0,
                    RecentJobs = recentJobs,
                    RecentCandidates = recentCandidates
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<BusinessDashboardDTO> GetBusinessDashboardByUserIdAsync(string userId)
        {
            try
            {
                var company = await _context.Companies
                    .Include(c => c.JobPostings)
                    .FirstOrDefaultAsync(c => c.UserId.ToString().ToLower() == userId);

                if (company == null)
                    return null;

                var today = DateTime.Today;
                var totalJobs = company.JobPostings?.Count ?? 0;
                var activeJobs = company.JobPostings?.Count(j => j.IsActive) ?? 0;
                var totalApplications = await GetTotalApplicationsForCompanyAsync(company.CompanyId);
                var todayApplications = await GetTodayApplicationsForCompanyAsync(company.CompanyId, today);
                var recentJobs = await GetRecentJobsAsync(company.CompanyId, company.CompanyName);
                var recentCandidates = await GetRecentCandidatesAsync(company.CompanyId);

                return new BusinessDashboardDTO
                {
                    TotalJobs = totalJobs,
                    ActiveJobs = activeJobs,
                    TotalApplications = totalApplications,
                    TotalViews = company.JobPostings?.Sum(jp => jp.Views) ?? 0,
                    TodayApplications = todayApplications,
                    TodayViews = 0,
                    RecentJobs = recentJobs,
                    RecentCandidates = recentCandidates
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<RecentJobDTO>> GetRecentJobsAsync(int businessId, string companyName)
        {
            return await _context.JobPostings
                .Where(jp => jp.CompanyId == businessId)
                .OrderByDescending(jp => jp.CreatedDate)
                .Take(5)
                .Select(jp => new RecentJobDTO
                {
                    Id = jp.JobId,
                    Title = jp.JobTitle,
                    Company = companyName,
                    Applications = _context.JobApplications.Count(ja => ja.JobId == jp.JobId),
                    Views = jp.Views,
                    PostedDate = jp.CreatedDate.ToString("yyyy-MM-dd"),
                    Status = jp.IsActive ? "active" : "inactive",
                    Urgent = jp.IsUrgent
                })
                .ToListAsync();
        }

        public async Task<List<RecentCandidateDTO>> GetRecentCandidatesAsync(int businessId)
        {
            return await _context.JobApplications
                .Where(ja => _context.JobPostings.Any(jp => jp.JobId == ja.JobId && jp.CompanyId == businessId))
                .Include(ja => ja.Worker)
                .ThenInclude(w => w.EducationLevel)
                .Include(ja => ja.Worker)
                .ThenInclude(w => w.District)
                .Include(ja => ja.JobPosting)
                .ThenInclude(jp => jp.Field)
                .OrderByDescending(ja => ja.AppliedDate)
                .Take(5)
                .Select(ja => new RecentCandidateDTO
                {
                    Id = ja.Worker.WorkerId,
                    Name = ja.Worker.FullName,
                    Position = ja.JobPosting.JobTitle,
                    Experience = ja.Worker.WorkExperience ?? "Chưa có kinh nghiệm",
                    Education = ja.Worker.EducationLevel != null ? ja.Worker.EducationLevel.LevelName : "Chưa cập nhật",
                    AppliedDate = ja.AppliedDate.ToString("yyyy-MM-dd"),
                    Status = ja.Status ?? "pending",
                    Avatar = ja.Worker.AvatarUrl ?? "",
                    Age = ja.Worker.DateOfBirth.HasValue ? DateTime.Today.Year - ja.Worker.DateOfBirth.Value.Year : 0,
                    Location = ja.Worker.District != null ? ja.Worker.District.DistrictName : "Chưa cập nhật",
                    Level = "Nhân viên",
                    Industry = ja.JobPosting.Field != null ? ja.JobPosting.Field.FieldName : "Chưa cập nhật",
                    PreviousCompany = ja.Worker.CurrentCompany ?? "Chưa cập nhật",
                    SalaryExpectation = ja.Worker.ExpectedSalary.HasValue ? ja.Worker.ExpectedSalary.Value.ToString("N0") + " VNĐ" : "Thương lượng"
                })
                .ToListAsync();
        }

        public async Task<int> GetTotalApplicationsForCompanyAsync(int companyId)
        {
            return await _context.JobApplications
                .Where(ja => _context.JobPostings.Any(jp => jp.JobId == ja.JobId && jp.CompanyId == companyId))
                .CountAsync();
        }

        public async Task<int> GetTodayApplicationsForCompanyAsync(int companyId, DateTime today)
        {
            return await _context.JobApplications
                .Where(ja => _context.JobPostings.Any(jp => jp.JobId == ja.JobId && jp.CompanyId == companyId) 
                            && ja.AppliedDate.Date == today)
                .CountAsync();
        }

        #endregion

        #region Candidate Dashboard Methods

        public async Task<CandidateDashboardDTO> GetCandidateDashboardAsync(string candidateId)
        {
            try
            {
                var worker = await _context.Workers
                    .Include(w => w.JobApplications)
                    .Include(w => w.SavedJobs)
                    .FirstOrDefaultAsync(w => w.UserId.ToString().ToLower() == candidateId.ToLower());

                if (worker == null)
                    return null;

                var totalApplications = worker.JobApplications?.Count ?? 0;
                var pendingApplications = worker.JobApplications?.Count(ja => ja.Status == "pending") ?? 0;
                var approvedApplications = worker.JobApplications?.Count(ja => ja.Status == "approved") ?? 0;
                var rejectedApplications = worker.JobApplications?.Count(ja => ja.Status == "rejected") ?? 0;

                var savedJobs = await GetSavedJobsAsync(candidateId);
                var recentApplications = await GetRecentApplicationsAsync(candidateId);
                var appliedJobs = await GetAppliedJobsAsync(candidateId);

                return new CandidateDashboardDTO
                {
                    TotalApplications = totalApplications,
                    PendingApplications = pendingApplications,
                    ApprovedApplications = approvedApplications,
                    RejectedApplications = rejectedApplications,
                    ProfileViews = 142,
                    SuitableJobs = await GetSuitableJobsCountAsync(candidateId),
                    EmployerEmails = 3,
                    TotalCVs = await GetTotalCVsAsync(candidateId),
                    SavedJobs = savedJobs,
                    RecentApplications = recentApplications,
                    AppliedJobs = appliedJobs
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<SavedJobDTO>> GetSavedJobsAsync(string candidateId)
        {
            return await _context.SavedJobs
                .Where(sj => sj.Worker.UserId.ToString().ToLower() == candidateId.ToLower())
                .Include(sj => sj.JobPosting)
                .ThenInclude(jp => jp.Company)
                .Include(sj => sj.Worker)
                .OrderByDescending(sj => sj.SavedDate)
                .Take(5)
                .Select(sj => new SavedJobDTO
                {
                    Id = sj.SavedJobId,
                    JobTitle = sj.JobPosting.JobTitle,
                    Company = sj.JobPosting.Company.CompanyName,
                    Logo = sj.JobPosting.Company.LogoUrl ?? "assets/vieclamlaocai/img/default-logo.png",
                    Salary = sj.JobPosting.MinSalary.HasValue && sj.JobPosting.MaxSalary.HasValue 
                        ? $"{sj.JobPosting.MinSalary.Value.ToString("N0")} - {sj.JobPosting.MaxSalary.Value.ToString("N0")} {(sj.JobPosting.SalaryType != null && sj.JobPosting.SalaryType.ToLower() == "monthly" ? "VNĐ/tháng" : "VNĐ")}"
                        : sj.JobPosting.MinSalary.HasValue 
                            ? $"Từ {sj.JobPosting.MinSalary.Value.ToString("N0")} {(sj.JobPosting.SalaryType != null && sj.JobPosting.SalaryType.ToLower() == "monthly" ? "VNĐ/tháng" : "VNĐ")}"
                            : sj.JobPosting.MaxSalary.HasValue 
                                ? $"Đến {sj.JobPosting.MaxSalary.Value.ToString("N0")} {(sj.JobPosting.SalaryType != null && sj.JobPosting.SalaryType.ToLower() == "monthly" ? "VNĐ/tháng" : "VNĐ")}"
                                : "Thương lượng",
                    SavedDate = sj.SavedDate.ToString("yyyy-MM-dd"),
                    Location = sj.JobPosting.District != null ? sj.JobPosting.District.DistrictName : "Chưa cập nhật",
                    Urgent = sj.JobPosting.IsUrgent
                })
                .ToListAsync();
        }

        public async Task<List<RecentApplicationDTO>> GetRecentApplicationsAsync(string candidateId)
        {
            return await _context.JobApplications
                .Where(ja => ja.Worker.UserId.ToString().ToLower() == candidateId.ToLower())
                .Include(ja => ja.JobPosting)
                .ThenInclude(jp => jp.Company)
                .Include(ja => ja.Worker)
                .OrderByDescending(ja => ja.AppliedDate)
                .Take(5)
                .Select(ja => new RecentApplicationDTO
                {
                    Id = ja.ApplicationId,
                    JobTitle = ja.JobPosting.JobTitle,
                    Company = ja.JobPosting.Company.CompanyName,
                    Logo = ja.JobPosting.Company.LogoUrl ?? "assets/vieclamlaocai/img/default-logo.png",
                    Salary = ja.JobPosting.MinSalary.HasValue && ja.JobPosting.MaxSalary.HasValue 
                        ? $"{ja.JobPosting.MinSalary.Value.ToString("N0")} - {ja.JobPosting.MaxSalary.Value.ToString("N0")} {(ja.JobPosting.SalaryType != null && ja.JobPosting.SalaryType.ToLower() == "monthly" ? "VNĐ/tháng" : "VNĐ")}"
                        : ja.JobPosting.MinSalary.HasValue 
                            ? $"Từ {ja.JobPosting.MinSalary.Value.ToString("N0")} {(ja.JobPosting.SalaryType != null && ja.JobPosting.SalaryType.ToLower() == "monthly" ? "VNĐ/tháng" : "VNĐ")}"
                            : ja.JobPosting.MaxSalary.HasValue 
                                ? $"Đến {ja.JobPosting.MaxSalary.Value.ToString("N0")} {(ja.JobPosting.SalaryType != null && ja.JobPosting.SalaryType.ToLower() == "monthly" ? "VNĐ/tháng" : "VNĐ")}"
                                : "Thương lượng",
                    Status = ja.Status ?? "pending",
                    AppliedDate = ja.AppliedDate.ToString("yyyy-MM-dd"),
                    Location = ja.JobPosting.District != null ? ja.JobPosting.District.DistrictName : "Chưa cập nhật"
                })
                .ToListAsync();
        }

        public async Task<List<AppliedJobDTO>> GetAppliedJobsAsync(string candidateId)
        {
            return await _context.JobApplications
                .Where(ja => ja.Worker.UserId.ToString().ToLower() == candidateId.ToLower())
                .Include(ja => ja.JobPosting)
                .ThenInclude(jp => jp.Company)
                .Include(ja => ja.Worker)
                .OrderByDescending(ja => ja.AppliedDate)
                .Select(ja => new AppliedJobDTO
                {
                    Id = ja.ApplicationId,
                    JobTitle = ja.JobPosting.JobTitle,
                    Company = ja.JobPosting.Company.CompanyName,
                    Logo = ja.JobPosting.Company.LogoUrl ?? "",
                    Salary = ja.JobPosting.MinSalary.HasValue && ja.JobPosting.MaxSalary.HasValue 
                        ? $"{ja.JobPosting.MinSalary.Value.ToString("N0")} - {ja.JobPosting.MaxSalary.Value.ToString("N0")} {(ja.JobPosting.SalaryType != null && ja.JobPosting.SalaryType.ToLower() == "monthly" ? "VNĐ/tháng" : "VNĐ")}"
                        : ja.JobPosting.MinSalary.HasValue 
                            ? $"Từ {ja.JobPosting.MinSalary.Value.ToString("N0")} {(ja.JobPosting.SalaryType != null && ja.JobPosting.SalaryType.ToLower() == "monthly" ? "VNĐ/tháng" : "VNĐ")}"
                            : ja.JobPosting.MaxSalary.HasValue 
                                ? $"Đến {ja.JobPosting.MaxSalary.Value.ToString("N0")} {(ja.JobPosting.SalaryType != null && ja.JobPosting.SalaryType.ToLower() == "monthly" ? "VNĐ/tháng" : "VNĐ")}"
                                : "Thương lượng",
                    Status = ja.Status ?? "pending",
                    AppliedDate = ja.AppliedDate.ToString("yyyy-MM-dd"),
                    Location = ja.JobPosting.District != null ? ja.JobPosting.District.DistrictName : "Chưa cập nhật"
                })
                .ToListAsync();
        }

        public async Task<int> GetSuitableJobsCountAsync(string workerId)
        {
            var worker = await _context.Workers
                .Include(w => w.Career)
                .FirstOrDefaultAsync(w => w.UserId.ToString().ToLower() == workerId.ToLower());

            if (worker == null || worker.CareerId == null)
                return 0;

            return await _context.JobPostings
                .CountAsync(jp => jp.CareerId == worker.CareerId && jp.IsActive);
        }

        public async Task<int> GetTotalCVsAsync(string workerId)
        {
            return await _context.CVs
                .Where(cv => cv.Worker.UserId.ToString().ToLower() == workerId.ToLower())
                .CountAsync();
        }

        #endregion

        #region Admin Dashboard Methods

        public async Task<AdminDashboardDTO> GetAdminDashboardAsync()
        {
            var today = DateTime.Today;

            var totalBusinesses = await GetTotalBusinessesAsync();
            var pendingApprovals = await GetPendingApprovalsAsync();
            var approvedBusinesses = await GetApprovedBusinessesAsync();
            var rejectedBusinesses = await GetRejectedBusinessesAsync();
            var totalJobs = await GetTotalJobsAsync();
            var totalCandidates = await GetTotalCandidatesAsync();
            var todayRegistrations = await GetTodayRegistrationsAsync(today);

            var systemHealth = new SystemHealthDTO
            {
                Status = "healthy",
                Uptime = "99.9%",
                ResponseTime = 150,
                ActiveUsers = await GetActiveUsersCountAsync()
            };

            return new AdminDashboardDTO
            {
                TotalBusinesses = totalBusinesses,
                PendingApprovals = pendingApprovals,
                ApprovedBusinesses = approvedBusinesses,
                RejectedBusinesses = rejectedBusinesses,
                TotalJobs = totalJobs,
                TotalCandidates = totalCandidates,
                TodayRegistrations = todayRegistrations,
                SystemHealth = systemHealth
            };
        }

        public async Task<int> GetTotalBusinessesAsync()
        {
            return await _context.Companies.CountAsync();
        }

        public async Task<int> GetPendingApprovalsAsync()
        {
            return await _context.BusinessApprovals
                .CountAsync(ba => ba.ApprovalStatus == "pending");
        }

        public async Task<int> GetApprovedBusinessesAsync()
        {
            return await _context.BusinessApprovals
                .CountAsync(ba => ba.ApprovalStatus == "approved");
        }

        public async Task<int> GetRejectedBusinessesAsync()
        {
            return await _context.BusinessApprovals
                .CountAsync(ba => ba.ApprovalStatus == "rejected");
        }

        public async Task<int> GetTotalJobsAsync()
        {
            return await _context.JobPostings.CountAsync();
        }

        public async Task<int> GetTotalCandidatesAsync()
        {
            return await _context.Workers.CountAsync();
        }

        public async Task<int> GetTodayRegistrationsAsync(DateTime today)
        {
            var companyRegistrations = await _context.Companies
                .CountAsync(c => c.CreatedDate.Date == today);
            var workerRegistrations = await _context.Workers
                .CountAsync(w => w.CreatedDate.Date == today);

            return companyRegistrations + workerRegistrations;
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            var last24Hours = DateTime.UtcNow.AddHours(-24);
            return await _context.UserActivity
                .Where(ua => ua.CreatedDate >= last24Hours)
                .Select(ua => ua.UserId)
                .Distinct()
                .CountAsync();
        }

        #endregion

        #region Activities Methods

        public async Task<List<ActivityDTO>> GetRecentActivitiesAsync(string userId)
        {
            var activities = await _context.UserActivity
                .Where(ua => ua.UserId == Guid.Parse(userId))
                .OrderByDescending(ua => ua.CreatedDate)
                .Take(10)
                .Select(ua => new
                {
                    ua.ActivityId,
                    ua.ActivityType,
                    ua.ActivityDescription,
                    ua.CreatedDate,
                    ua.UserId
                })
                .ToListAsync();

            return activities.Select(ua => new ActivityDTO
            {
                Id = ua.ActivityId,
                Type = MapActivityType(ua.ActivityType),
                Title = ua.ActivityType,
                Description = ua.ActivityDescription,
                Timestamp = ua.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UserId = ua.UserId.ToString()
            }).ToList();
        }

        #endregion

        #region Private Helper Methods

        private string MapActivityType(string activityType)
        {
            if (activityType == null)
                return "application";
                
            return activityType.ToLower() switch
            {
                "login" => "application",
                "job_view" => "job_view",
                "profile_update" => "profile_update",
                "message" => "message",
                _ => "application"
            };
        }

        #endregion
    }
}
