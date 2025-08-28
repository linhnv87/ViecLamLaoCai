using System;
using System.Collections.Generic;

namespace Core.DTOs
{
    #region Dashboard DTOs

    public class BusinessDashboardDTO
    {
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int TotalApplications { get; set; }
        public int TotalViews { get; set; }
        public int TodayApplications { get; set; }
        public int TodayViews { get; set; }
        public List<RecentJobDTO> RecentJobs { get; set; } = new List<RecentJobDTO>();
        public List<RecentCandidateDTO> RecentCandidates { get; set; } = new List<RecentCandidateDTO>();
    }

    public class CandidateDashboardDTO
    {
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int ProfileViews { get; set; }
        public int SuitableJobs { get; set; }
        public int EmployerEmails { get; set; }
        public int TotalCVs { get; set; }
        public List<SavedJobDTO> SavedJobs { get; set; } = new List<SavedJobDTO>();
        public List<RecentApplicationDTO> RecentApplications { get; set; } = new List<RecentApplicationDTO>();
        public List<AppliedJobDTO> AppliedJobs { get; set; } = new List<AppliedJobDTO>();
    }

    public class AdminDashboardDTO
    {
        public int TotalBusinesses { get; set; }
        public int PendingApprovals { get; set; }
        public int ApprovedBusinesses { get; set; }
        public int RejectedBusinesses { get; set; }
        public int TotalJobs { get; set; }
        public int TotalCandidates { get; set; }
        public int TodayRegistrations { get; set; }
        public SystemHealthDTO SystemHealth { get; set; } = new SystemHealthDTO();
    }

    #endregion

    #region Detail DTOs

    public class RecentJobDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public int Applications { get; set; }
        public int Views { get; set; }
        public string PostedDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool Urgent { get; set; }
    }

    public class RecentCandidateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string AppliedDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string PreviousCompany { get; set; } = string.Empty;
        public string SalaryExpectation { get; set; } = string.Empty;
    }

    public class SavedJobDTO
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Salary { get; set; } = string.Empty;
        public string SavedDate { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool Urgent { get; set; }
    }

    public class RecentApplicationDTO
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Salary { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AppliedDate { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class AppliedJobDTO
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Salary { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AppliedDate { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class ActivityDTO
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    public class SystemHealthDTO
    {
        public string Status { get; set; } = string.Empty;
        public string Uptime { get; set; } = string.Empty;
        public int ResponseTime { get; set; }
        public int ActiveUsers { get; set; }
    }

    #endregion
}




