using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Database;
using Services.DTO;
using Database.Models;
using Database.Models.Website;

namespace Services
{
    public interface IHomeService
    {
        Task<HomePageDataDTO> GetHomePageDataAsync();
        Task<List<FeaturedJobDTO>> GetFeaturedJobsAsync(int? count = null);
        Task<List<SuggestedJobDTO>> GetSuggestedJobsAsync(string userId = null, int? count = null);
        Task<List<JobCategoryDTO>> GetJobCategoriesAsync();
        Task<HomePageStatsDTO> GetHomeStatsAsync();
        Task<List<LatestJobDTO>> GetLatestJobsAsync(int? count = null);
        Task<List<FeaturedCompanyDTO>> GetFeaturedCompaniesAsync(int? count = null);
        Task<PaginatedResponseDTO<FeaturedJobDTO>> SearchJobsAsync(SearchJobsRequestDTO searchQuery);
        Task<List<string>> GetPopularSearchesAsync(int? count = null, string period = null);
    }

    public class HomeService : IHomeService
    {
        private readonly QLTTrContext _context;
        private readonly ILogger<HomeService> _logger;

        public HomeService(QLTTrContext context, ILogger<HomeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HomePageDataDTO> GetHomePageDataAsync()
        {
            try
            {
                _logger.LogInformation("Getting home page data");

                var stats = await GetHomeStatsAsync();
                var featuredJobs = await GetFeaturedJobsAsync(6);
                var suggestedJobs = await GetSuggestedJobsAsync(null, 6);
                var jobCategories = await GetJobCategoriesAsync();
                var latestJobs = await GetLatestJobsAsync(8);
                var featuredCompanies = await GetFeaturedCompaniesAsync(7);

                return new HomePageDataDTO
                {
                    Stats = stats,
                    FeaturedJobs = featuredJobs,
                    SuggestedJobs = suggestedJobs,
                    JobCategories = jobCategories,
                    LatestJobs = latestJobs,
                    FeaturedCompanies = featuredCompanies
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting home page data");
                throw new Exception("Lỗi xảy ra khi lấy dữ liệu trang chủ: " + ex.Message);
            }
        }

        /// <summary>
        /// API TUYỂN GẤP - Lấy danh sách việc làm có IsUrgent = true
        /// Hiển thị các công việc cần tuyển gấp trong section "Tuyển gấp"
        /// </summary>
        public async Task<List<FeaturedJobDTO>> GetFeaturedJobsAsync(int? count = null)
        {
            try
            {
                _logger.LogInformation("Getting urgent jobs (tuyển gấp), count: {Count}", count);

                var query = _context.JobPostings
                    .Include(j => j.Company)
                    .Include(j => j.Field)
                    .Include(j => j.Career)
                    .Where(j => j.IsUrgent && j.IsActive) // Chỉ lấy việc làm TUYỂN GẤP
                    .OrderByDescending(j => j.CreatedDate);

                var jobs = count.HasValue 
                    ? await query.Take(count.Value).ToListAsync()
                    : await query.ToListAsync();

                return jobs.Select(job => new FeaturedJobDTO
                {
                    Id = job.JobId,
                    Title = job.JobTitle,
                    Company = job.Company?.CompanyName ?? "Unknown Company",
                    Logo = job.Company?.LogoUrl ?? "assets/vieclamlaocai/img/image 16.png",
                    Salary = FormatSalary(job.MinSalary, job.MaxSalary, job.SalaryType),
                    Location = job.WorkLocation ?? "Lào Cai",
                    Urgent = job.IsUrgent, // Luôn true cho API này
                    DaysLeft = CalculateDaysLeft(job.CreatedDate),
                    Featured = job.IsFeatured,
                    JobType = job.EmploymentType ?? "Full-time",
                    Experience = "2-3 năm", // Default value since not in model
                    PostedDate = job.CreatedDate.ToString("yyyy-MM-dd"),
                    Description = job.JobDescription ?? "",
                    Requirements = ParseRequirements(job.Requirements),
                    Benefits = ParseBenefits(job.Benefits)
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting urgent jobs");
                throw new Exception("Lỗi xảy ra khi lấy việc làm tuyển gấp: " + ex.Message);
            }
        }

        /// <summary>
        /// API GỢI Ý PHÙ HỢP - Match CareerId của Worker với JobPosting
        /// Tìm việc làm phù hợp với ngành nghề của user trong section "Gợi ý việc làm phù hợp với bạn"
        /// </summary>
        public async Task<List<SuggestedJobDTO>> GetSuggestedJobsAsync(string userId = null, int? count = null)
        {
            try
            {
                _logger.LogInformation("Getting suggested jobs for user: {UserId}, count: {Count}", userId, count);

                IQueryable<JobPosting> query;

                if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
                {
                    // Lấy thông tin Worker của user để match career
                    var worker = await _context.Workers
                        .Include(w => w.Career)
                        .ThenInclude(c => c.Specialization)
                        .Where(w => w.UserId == userGuid && w.IsActive)
                        .FirstOrDefaultAsync();

                    if (worker != null && worker.CareerId.HasValue)
                    {
                        // Gợi ý job cùng Career hoặc cùng Specialization
                        query = _context.JobPostings
                            .Include(j => j.Company)
                            .Include(j => j.Career)
                            .Include(j => j.Field)
                            .Where(j => j.IsActive && 
                                   (j.CareerId == worker.CareerId || // Cùng nghề nghiệp
                                    (j.Career != null && j.Career.Specialization != null && 
                                     worker.Career != null && worker.Career.SpecializationId == j.Career.SpecializationId))) // Cùng chuyên ngành
                            .OrderByDescending(j => j.CreatedDate);
                    }
                    else
                    {
                        // Fallback: Lấy job mới nhất nếu không có thông tin career
                        query = _context.JobPostings
                            .Include(j => j.Company)
                            .Where(j => j.IsActive)
                            .OrderByDescending(j => j.CreatedDate);
                    }
                }
                else
                {
                    // Guest user: Lấy job mới nhất và đa dạng
                    query = _context.JobPostings
                        .Include(j => j.Company)
                        .Where(j => j.IsActive)
                        .OrderByDescending(j => j.CreatedDate);
                }

                var jobs = count.HasValue 
                    ? await query.Take(count.Value).ToListAsync()
                    : await query.ToListAsync();

                return jobs.Select(job => new SuggestedJobDTO
                {
                    Id = job.JobId,
                    Title = job.JobTitle,
                    Company = job.Company?.CompanyName ?? "Unknown Company",
                    Logo = job.Company?.LogoUrl ?? "assets/vieclamlaocai/img/image 23.png",
                    Salary = FormatSalary(job.MinSalary, job.MaxSalary, job.SalaryType),
                    Experience = "2-3 năm kinh nghiệm", // Default value
                    Location = job.WorkLocation ?? "Lào Cai",
                    JobType = job.EmploymentType ?? "Full-time",
                    PostedDate = job.CreatedDate.ToString("yyyy-MM-dd"),
                    Description = job.JobDescription ?? "",
                    Requirements = ParseRequirements(job.Requirements)
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting suggested jobs");
                throw new Exception("Lỗi xảy ra khi lấy việc làm gợi ý: " + ex.Message);
            }
        }

        /// <summary>
        /// API TOP NGÀNH NGHỀ NỔI BẬT - Lấy từ Fields table và đếm số JobPosting
        /// Hiển thị tên ngành và số lượng job của ngành đó trong section "Top ngành nghề nổi bật"
        /// </summary>
        public async Task<List<JobCategoryDTO>> GetJobCategoriesAsync()
        {
            try
            {
                _logger.LogInformation("Getting job categories from Fields table");

                // Lấy data thực từ Fields table và đếm số job
                var fieldsWithJobCount = await _context.Fields
                    .Where(f => f.IsActive)
                    .Select(f => new
                    {
                        Field = f,
                        JobCount = f.JobPostings.Count(j => j.IsActive)
                    })
                    .Where(x => x.JobCount > 0) // Chỉ lấy field có job
                    .OrderByDescending(x => x.JobCount)
                    .Take(6) // Top 6 ngành nghề nổi bật
                    .ToListAsync();

                // Map với icon tương ứng
                var iconMapping = new Dictionary<string, string>
                {
                    {"Công nghệ thông tin", "icon-color-ai.svg"},
                    {"Kinh doanh", "icon-bags.svg"},
                    {"Tài chính", "icon-color-calculator.svg"},
                    {"Bất động sản", "icon-color-town.svg"},
                    {"Hành chính", "icon-color-folder.svg"},
                    {"Xây dựng", "icon-color-hat.svg"},
                    {"Y tế", "icon-heart.svg"},
                    {"Giáo dục", "icon-bags.svg"}
                };

                var categories = fieldsWithJobCount.Select(item => new JobCategoryDTO
                {
                    Icon = iconMapping.ContainsKey(item.Field.FieldName) 
                           ? iconMapping[item.Field.FieldName] 
                           : "icon-bags.svg", // Default icon
                    Title = item.Field.FieldName,
                    Count = item.JobCount,
                    GrowthRate = 12.5, // Có thể tính toán growth rate thực nếu cần
                    AverageSalary = "10-25 triệu" // Có thể tính average salary thực nếu cần
                }).ToList();

                // Nếu không có data thực, fallback sang mock data
                if (!categories.Any())
                {
                    categories = new List<JobCategoryDTO>
                    {
                        new JobCategoryDTO { Icon = "icon-bags.svg", Title = "Kinh doanh - Bán hàng", Count = 0, GrowthRate = 12.5, AverageSalary = "12-25 triệu" },
                        new JobCategoryDTO { Icon = "icon-color-calculator.svg", Title = "Tài chính - Kế toán", Count = 0, GrowthRate = 8.2, AverageSalary = "10-20 triệu" },
                        new JobCategoryDTO { Icon = "icon-color-town.svg", Title = "Bất động sản", Count = 0, GrowthRate = 15.8, AverageSalary = "15-35 triệu" },
                        new JobCategoryDTO { Icon = "icon-color-ai.svg", Title = "Công nghệ thông tin", Count = 0, GrowthRate = 25.3, AverageSalary = "18-40 triệu" },
                        new JobCategoryDTO { Icon = "icon-color-folder.svg", Title = "Hành chính - Thư ký", Count = 0, GrowthRate = 5.1, AverageSalary = "8-15 triệu" },
                        new JobCategoryDTO { Icon = "icon-color-hat.svg", Title = "Xây dựng", Count = 0, GrowthRate = 18.7, AverageSalary = "12-28 triệu" }
                    };
                }

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting job categories");
                throw new Exception("Lỗi xảy ra khi lấy danh mục việc làm: " + ex.Message);
            }
        }

        public async Task<HomePageStatsDTO> GetHomeStatsAsync()
        {
            try
            {
                _logger.LogInformation("Getting home page stats");

                var totalJobs = await _context.JobPostings.CountAsync(j => j.IsActive);
                var totalCompanies = await _context.Companies.CountAsync(c => c.IsActive);
                var totalCandidates = await _context.Workers.CountAsync(w => w.IsActive);
                var newJobsThisWeek = await _context.JobPostings.CountAsync(j => j.IsActive && j.CreatedDate >= DateTime.Now.AddDays(-7));
                var featuredJobsCount = await _context.JobPostings.CountAsync(j => j.IsActive && j.IsFeatured);
                var urgentJobsCount = await _context.JobPostings.CountAsync(j => j.IsActive && j.IsUrgent);

                return new HomePageStatsDTO
                {
                    TotalJobs = totalJobs,
                    TotalCompanies = totalCompanies,
                    TotalCandidates = totalCandidates,
                    NewJobsThisWeek = newJobsThisWeek,
                    FeaturedJobsCount = featuredJobsCount,
                    UrgentJobsCount = urgentJobsCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting home page stats");
                throw new Exception("Lỗi xảy ra khi lấy thống kê trang chủ: " + ex.Message);
            }
        }

        /// <summary>
        /// API VIỆC LÀM MỚI NHẤT - Sắp xếp theo CreatedDate mới nhất
        /// Hiển thị công việc theo thời gian tạo mới nhất trong section "Việc làm mới nhất"
        /// </summary>
        public async Task<List<LatestJobDTO>> GetLatestJobsAsync(int? count = null)
        {
            try
            {
                _logger.LogInformation("Getting latest jobs by CreatedDate, count: {Count}", count);

                var query = _context.JobPostings
                    .Include(j => j.Company)
                    .Include(j => j.Field)
                    .Include(j => j.Career)
                    .Where(j => j.IsActive) // Chỉ lấy job đang active
                    .OrderByDescending(j => j.CreatedDate); // Sắp xếp theo mới nhất

                var jobs = count.HasValue 
                    ? await query.Take(count.Value).ToListAsync()
                    : await query.ToListAsync();

                return jobs.Select(job => new LatestJobDTO
                {
                    Id = job.JobId,
                    Title = job.JobTitle,
                    Company = job.Company?.CompanyName ?? "Unknown Company",
                    Logo = job.Company?.LogoUrl ?? "assets/vieclamlaocai/img/image 16.png",
                    Salary = FormatSalary(job.MinSalary, job.MaxSalary, job.SalaryType),
                    Location = job.WorkLocation ?? "Lào Cai",
                    Urgent = job.IsUrgent,
                    DaysLeft = CalculateDaysLeft(job.CreatedDate),
                    PostedDate = job.CreatedDate.ToString("yyyy-MM-dd"),
                    JobType = job.EmploymentType ?? "Full-time",
                    Experience = "2-3 năm" // Default value
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest jobs");
                throw new Exception("Lỗi xảy ra khi lấy việc làm mới nhất: " + ex.Message);
            }
        }

        /// <summary>
        /// API CÔNG TY NỔI BẬT - Top công ty có nhiều JobPosting và verified
        /// Lấy top những công ty có nhiều job posting nhất và đã verified trong section "Công ty nổi bật"
        /// </summary>
        public async Task<List<FeaturedCompanyDTO>> GetFeaturedCompaniesAsync(int? count = null)
        {
            try
            {
                _logger.LogInformation("Getting top featured companies by job count and verified status, count: {Count}", count);

                // Lấy companies với job count thực và ưu tiên verified companies
                var companiesWithJobCount = await _context.Companies
                    .Where(c => c.IsActive)
                    .Select(c => new
                    {
                        Company = c,
                        ActiveJobCount = c.JobPostings.Count(j => j.IsActive), // Đếm job đang active
                        TotalJobCount = c.JobPostings.Count() // Tổng số job đã đăng
                    })
                    .Where(x => x.ActiveJobCount > 0 || x.Company.IsVerified) // Có job hoặc đã verified
                    .OrderByDescending(x => x.Company.IsVerified) // Ưu tiên verified companies
                    .ThenByDescending(x => x.ActiveJobCount) // Sau đó sắp xếp theo số job
                    .ThenByDescending(x => x.TotalJobCount) // Cuối cùng theo tổng job
                    .ToListAsync();

                if (count.HasValue)
                {
                    companiesWithJobCount = companiesWithJobCount.Take(count.Value).ToList();
                }

                return companiesWithJobCount.Select(item => new FeaturedCompanyDTO
                {
                    Id = item.Company.CompanyId,
                    Name = item.Company.CompanyName,
                    Logo = item.Company.LogoUrl ?? "",
                    JobCount = item.ActiveJobCount,
                    Industry = item.Company.Industry ?? "",
                    Size = item.Company.CompanySize ?? "",
                    Location = item.Company.Address ?? "",
                    Verified = item.Company.IsVerified,
                    Description = item.Company.Description ?? "",
                    Website = item.Company.Website ?? ""
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured companies");
                throw new Exception("Lỗi xảy ra khi lấy công ty nổi bật: " + ex.Message);
            }
        }

        /// <summary>
        /// API TÌM KIẾM VIỆC LÀM VỚI PHÂN TRANG - Tìm kiếm job theo nhiều tiêu chí với pagination
        /// </summary>
        public async Task<PaginatedResponseDTO<FeaturedJobDTO>> SearchJobsAsync(SearchJobsRequestDTO searchQuery)
        {
            try
            {
                var startTime = DateTime.Now;
                _logger.LogInformation("Searching jobs with pagination: {@SearchQuery}", searchQuery);

                // Validate pagination parameters
                if (searchQuery.Page < 1) searchQuery.Page = 1;
                if (searchQuery.PageSize < 1 || searchQuery.PageSize > 50) searchQuery.PageSize = 10;

                var query = _context.JobPostings
                    .Include(j => j.Company)
                    .Include(j => j.Field)
                    .Include(j => j.Career)
                    .Where(j => j.IsActive);

                // Tìm kiếm theo từ khóa (title, description, company name)
                if (!string.IsNullOrEmpty(searchQuery.Keyword))
                {
                    var keyword = searchQuery.Keyword.ToLower();
                    query = query.Where(j => 
                        j.JobTitle.ToLower().Contains(keyword) ||
                        (j.JobDescription != null && j.JobDescription.ToLower().Contains(keyword)) ||
                        (j.Company != null && j.Company.CompanyName.ToLower().Contains(keyword)) ||
                        (j.Field != null && j.Field.FieldName.ToLower().Contains(keyword)) ||
                        (j.Career != null && j.Career.CareerName.ToLower().Contains(keyword)));
                }

                // Tìm kiếm theo địa điểm
                if (!string.IsNullOrEmpty(searchQuery.Location))
                {
                    query = query.Where(j => j.WorkLocation != null && j.WorkLocation.Contains(searchQuery.Location));
                }

                // Tìm kiếm theo category (Field)
                if (!string.IsNullOrEmpty(searchQuery.Category))
                {
                    query = query.Where(j => j.Field != null && j.Field.FieldName.Contains(searchQuery.Category));
                }

                // Lọc theo loại công việc
                if (!string.IsNullOrEmpty(searchQuery.JobType))
                {
                    query = query.Where(j => j.EmploymentType == searchQuery.JobType);
                }

                // Lọc theo mức lương
                if (!string.IsNullOrEmpty(searchQuery.SalaryRange))
                {
                    var salaryParts = searchQuery.SalaryRange.Split('-');
                    if (salaryParts.Length == 2)
                    {
                        if (decimal.TryParse(salaryParts[0], out var minSalary) && 
                            decimal.TryParse(salaryParts[1], out var maxSalary))
                        {
                            query = query.Where(j => 
                                (j.MinSalary >= minSalary && j.MinSalary <= maxSalary) ||
                                (j.MaxSalary >= minSalary && j.MaxSalary <= maxSalary) ||
                                (j.MinSalary <= minSalary && j.MaxSalary >= maxSalary));
                        }
                    }
                    else if (searchQuery.SalaryRange.EndsWith("+"))
                    {
                        var minStr = searchQuery.SalaryRange.Replace("+", "");
                        if (decimal.TryParse(minStr, out var minSalary))
                        {
                            query = query.Where(j => j.MinSalary >= minSalary || j.MaxSalary >= minSalary);
                        }
                    }
                }

                IOrderedQueryable<JobPosting> orderedQuery;
                switch (searchQuery.SortBy?.ToLower())
                {
                    case "date":
                        orderedQuery = searchQuery.SortOrder?.ToLower() == "asc" 
                            ? query.OrderBy(j => j.CreatedDate)
                            : query.OrderByDescending(j => j.CreatedDate);
                        break;
                    case "salary":
                        orderedQuery = searchQuery.SortOrder?.ToLower() == "asc"
                            ? query.OrderBy(j => j.MaxSalary ?? j.MinSalary ?? 0)
                            : query.OrderByDescending(j => j.MaxSalary ?? j.MinSalary ?? 0);
                        break;
                    case "company":
                        orderedQuery = searchQuery.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(j => j.Company.CompanyName)
                            : query.OrderBy(j => j.Company.CompanyName);
                        break;
                    default:
                        orderedQuery = query
                            .OrderByDescending(j => j.IsUrgent)
                            .ThenByDescending(j => j.IsFeatured)
                            .ThenByDescending(j => j.CreatedDate);
                        break;
                }

                var totalItems = await orderedQuery.CountAsync();
                var skip = (searchQuery.Page - 1) * searchQuery.PageSize;
                var jobs = await orderedQuery
                    .Skip(skip)
                    .Take(searchQuery.PageSize)
                    .ToListAsync();

                var searchTime = (int)(DateTime.Now - startTime).TotalMilliseconds;
                var totalPages = (int)Math.Ceiling((double)totalItems / searchQuery.PageSize);

                _logger.LogInformation("Found {TotalItems} jobs, returning page {CurrentPage}/{TotalPages} ({Count} items) in {SearchTime}ms", 
                    totalItems, searchQuery.Page, totalPages, jobs.Count, searchTime);

                var response = new PaginatedResponseDTO<FeaturedJobDTO>
                {
                    Data = jobs.Select(job => new FeaturedJobDTO
                    {
                        Id = job.JobId,
                        Title = job.JobTitle,
                        Company = job.Company?.CompanyName ?? "Unknown Company",
                        Logo = job.Company?.LogoUrl ?? "assets/vieclamlaocai/img/image 16.png",
                        Salary = FormatSalary(job.MinSalary, job.MaxSalary, job.SalaryType),
                        Location = job.WorkLocation ?? "Lào Cai",
                        Urgent = job.IsUrgent,
                        DaysLeft = CalculateDaysLeft(job.CreatedDate),
                        Featured = job.IsFeatured,
                        JobType = job.EmploymentType ?? "Full-time",
                        Experience = "2-3 năm", // Default value
                        PostedDate = job.CreatedDate.ToString("yyyy-MM-dd"),
                        Description = job.JobDescription ?? "",
                        Requirements = ParseRequirements(job.Requirements),
                        Benefits = ParseBenefits(job.Benefits)
                    }).ToList(),
                    CurrentPage = searchQuery.Page,
                    PageSize = searchQuery.PageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasPreviousPage = searchQuery.Page > 1,
                    HasNextPage = searchQuery.Page < totalPages,
                    SearchQuery = searchQuery.Keyword ?? "",
                    SearchTime = searchTime
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching jobs with pagination: {Error}", ex.Message);
                throw new Exception("Lỗi xảy ra khi tìm kiếm việc làm có phân trang: " + ex.Message);
            }
        }

        public async Task<List<string>> GetPopularSearchesAsync(int? count = null, string period = null)
        {
            try
            {
                _logger.LogInformation("Getting popular searches, count: {Count}, period: {Period}", count, period);

                // Mock popular searches - in real implementation, this would come from search logs
                var popularSearches = new List<string>
                {
                    "Frontend Developer",
                    "Backend Developer",
                    "Full Stack Developer",
                    "UI/UX Designer",
                    "Digital Marketing",
                    "Kế toán",
                    "Nhân viên bán hàng",
                    "Project Manager"
                };

                if (count.HasValue)
                {
                    popularSearches = popularSearches.Take(count.Value).ToList();
                }

                return popularSearches;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular searches");
                throw new Exception("Lỗi xảy ra khi lấy tìm kiếm phổ biến: " + ex.Message);
            }
        }

        private int CalculateDaysLeft(DateTime postedDate)
        {
            var daysSincePosted = (DateTime.Now - postedDate).Days;
            return Math.Max(0, 30 - daysSincePosted); // Assume 30 days job posting period
        }

        private string FormatSalary(decimal? minSalary, decimal? maxSalary, string salaryType)
        {
            if (!minSalary.HasValue && !maxSalary.HasValue)
                return "Thỏa thuận";

            var min = minSalary?.ToString("N0") ?? "0";
            var max = maxSalary?.ToString("N0") ?? "0";
            var type = salaryType?.ToLower() == "monthly" ? "triệu/tháng" : "triệu";

            if (minSalary.HasValue && maxSalary.HasValue)
                return $"{min} - {max} {type}";
            else if (minSalary.HasValue)
                return $"Từ {min} {type}";
            else
                return $"Đến {max} {type}";
        }

        private List<string> ParseRequirements(string requirements)
        {
            if (string.IsNullOrEmpty(requirements))
                return new List<string>();

            return requirements.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .ToList();
        }

        private List<string> ParseBenefits(string benefits)
        {
            if (string.IsNullOrEmpty(benefits))
                return new List<string>();

            return benefits.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(b => b.Trim())
                .ToList();
        }
    }
}
