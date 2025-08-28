using Repositories;
using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IDashboardService
    {
        // Main Dashboard APIs
        Task<BusinessDashboardDTO> GetBusinessDashboardAsync(int businessId);
        Task<BusinessDashboardDTO> GetBusinessDashboardByUserIdAsync(string userId);
        Task<CandidateDashboardDTO> GetCandidateDashboardAsync(string candidateId);
        Task<AdminDashboardDTO> GetAdminDashboardAsync();
        Task<List<ActivityDTO>> GetRecentActivitiesAsync(string userId);
        
        // Detailed APIs for Business Dashboard
        Task<List<RecentJobDTO>> GetRecentJobsAsync(int businessId, string companyName);
        Task<List<RecentCandidateDTO>> GetRecentCandidatesAsync(int businessId);
        
        // Detailed APIs for Candidate Dashboard
        Task<List<SavedJobDTO>> GetSavedJobsAsync(string candidateId);
        Task<List<RecentApplicationDTO>> GetRecentApplicationsAsync(string candidateId);
        Task<List<AppliedJobDTO>> GetAppliedJobsAsync(string candidateId);
        
        // Admin Dashboard Details
        Task<int> GetTotalBusinessesAsync();
        Task<int> GetPendingApprovalsAsync();
        Task<int> GetApprovedBusinessesAsync();
        Task<int> GetRejectedBusinessesAsync();
        Task<int> GetTotalJobsAsync();
        Task<int> GetTotalCandidatesAsync();
        Task<int> GetTodayRegistrationsAsync();
        Task<int> GetActiveUsersCountAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<BusinessDashboardDTO> GetBusinessDashboardAsync(int businessId)
        {
            var dashboard = await _dashboardRepository.GetBusinessDashboardAsync(businessId);
            
            if (dashboard == null)
            {
                throw new Exception("Business not found");
            }

            return dashboard;
        }

        public async Task<BusinessDashboardDTO> GetBusinessDashboardByUserIdAsync(string userId)
        {
            var dashboard = await _dashboardRepository.GetBusinessDashboardByUserIdAsync(userId);
            
            if (dashboard == null)
            {
                throw new Exception("Business not found for user");
            }

            return dashboard;
        }

        public async Task<CandidateDashboardDTO> GetCandidateDashboardAsync(string candidateId)
        {
            var dashboard = await _dashboardRepository.GetCandidateDashboardAsync(candidateId);
            
            if (dashboard == null)
            {
                throw new Exception("Candidate not found");
            }

            return dashboard;
        }

        public async Task<AdminDashboardDTO> GetAdminDashboardAsync()
        {
            return await _dashboardRepository.GetAdminDashboardAsync();
        }

        public async Task<List<ActivityDTO>> GetRecentActivitiesAsync(string userId)
        {
            return await _dashboardRepository.GetRecentActivitiesAsync(userId);
        }

        // Detailed APIs for Business Dashboard
        public async Task<List<RecentJobDTO>> GetRecentJobsAsync(int businessId, string companyName)
        {
            return await _dashboardRepository.GetRecentJobsAsync(businessId, companyName);
        }

        public async Task<List<RecentCandidateDTO>> GetRecentCandidatesAsync(int businessId)
        {
            return await _dashboardRepository.GetRecentCandidatesAsync(businessId);
        }

        // Detailed APIs for Candidate Dashboard
        public async Task<List<SavedJobDTO>> GetSavedJobsAsync(string candidateId)
        {
            return await _dashboardRepository.GetSavedJobsAsync(candidateId);
        }

        public async Task<List<RecentApplicationDTO>> GetRecentApplicationsAsync(string candidateId)
        {
            return await _dashboardRepository.GetRecentApplicationsAsync(candidateId);
        }

        public async Task<List<AppliedJobDTO>> GetAppliedJobsAsync(string candidateId)
        {
            return await _dashboardRepository.GetAppliedJobsAsync(candidateId);
        }

        // Admin Dashboard Details
        public async Task<int> GetTotalBusinessesAsync()
        {
            return await _dashboardRepository.GetTotalBusinessesAsync();
        }

        public async Task<int> GetPendingApprovalsAsync()
        {
            return await _dashboardRepository.GetPendingApprovalsAsync();
        }

        public async Task<int> GetApprovedBusinessesAsync()
        {
            return await _dashboardRepository.GetApprovedBusinessesAsync();
        }

        public async Task<int> GetRejectedBusinessesAsync()
        {
            return await _dashboardRepository.GetRejectedBusinessesAsync();
        }

        public async Task<int> GetTotalJobsAsync()
        {
            return await _dashboardRepository.GetTotalJobsAsync();
        }

        public async Task<int> GetTotalCandidatesAsync()
        {
            return await _dashboardRepository.GetTotalCandidatesAsync();
        }

        public async Task<int> GetTodayRegistrationsAsync()
        {
            return await _dashboardRepository.GetTodayRegistrationsAsync(DateTime.Today);
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _dashboardRepository.GetActiveUsersCountAsync();
        }
    }
}
