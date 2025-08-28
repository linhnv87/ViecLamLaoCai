using System.Collections.Generic;

namespace Services.DTO
{
    public class FeaturedJobDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Logo { get; set; }
        public string Salary { get; set; }
        public string Location { get; set; }
        public bool Urgent { get; set; }
        public int DaysLeft { get; set; }
        public bool Featured { get; set; }
        public string JobType { get; set; }
        public string Experience { get; set; }
        public string PostedDate { get; set; }
        public string Description { get; set; }
        public List<string> Requirements { get; set; }
        public List<string> Benefits { get; set; }
    }

    public class SuggestedJobDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Logo { get; set; }
        public string Salary { get; set; }
        public string Experience { get; set; }
        public string Location { get; set; }
        public string JobType { get; set; }
        public string PostedDate { get; set; }
        public string Description { get; set; }
        public List<string> Requirements { get; set; }
    }

    public class JobCategoryDTO
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
        public double GrowthRate { get; set; }
        public string AverageSalary { get; set; }
    }

    public class HomePageStatsDTO
    {
        public int TotalJobs { get; set; }
        public int TotalCompanies { get; set; }
        public int TotalCandidates { get; set; }
        public int NewJobsThisWeek { get; set; }
        public int FeaturedJobsCount { get; set; }
        public int UrgentJobsCount { get; set; }
    }

    public class LatestJobDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Logo { get; set; }
        public string Salary { get; set; }
        public string Location { get; set; }
        public bool Urgent { get; set; }
        public int DaysLeft { get; set; }
        public string PostedDate { get; set; }
        public string JobType { get; set; }
        public string Experience { get; set; }
    }

    public class FeaturedCompanyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public int JobCount { get; set; }
        public string Industry { get; set; }
        public string Size { get; set; }
        public string Location { get; set; }
        public bool Verified { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
    }

    public class HomePageDataDTO
    {
        public HomePageStatsDTO Stats { get; set; }
        public List<FeaturedJobDTO> FeaturedJobs { get; set; }
        public List<SuggestedJobDTO> SuggestedJobs { get; set; }
        public List<JobCategoryDTO> JobCategories { get; set; }
        public List<LatestJobDTO> LatestJobs { get; set; }
        public List<FeaturedCompanyDTO> FeaturedCompanies { get; set; }
    }

    public class SearchJobsRequestDTO
    {
        public string? Keyword { get; set; }
        public string? Location { get; set; }
        public string? Category { get; set; }  // Optional category field
        public string? SalaryRange { get; set; }
        public string? Experience { get; set; }
        public string? JobType { get; set; }
        
        // Pagination parameters
        public int Page { get; set; } = 1;          // Current page (default: 1)
        public int PageSize { get; set; } = 10;     // Items per page (default: 10)
        public string? SortBy { get; set; }         // Sort field: relevance, date, salary, company
        public string? SortOrder { get; set; }      // Sort order: asc, desc
    }

    public class PaginatedResponseDTO<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public string SearchQuery { get; set; } = "";
        public int SearchTime { get; set; }  // milliseconds
    }
}
