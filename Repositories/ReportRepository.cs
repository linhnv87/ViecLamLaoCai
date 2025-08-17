using Database.Models;
using Database.STPCModels;
using Microsoft.Data.SqlClient;
using Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories;

public interface IReportRepository
{
    Task<(List<ReviewReportSTPC>, int, List<AppUserRoles>)> GetReviewReportAsync(string? keyword, string? fromDate, string? toDate, int? reviewResult, int? submitCount, int pageNumber, int pageSize, string? sortColumn, string? sortOrder);

    Task<(List<ReportApprovalByUserSTPC>, int, List<AppUserRoles>)> GetReportApprovalByUserAsync(string? keyword, string? fromDate, string? toDate, int pageNumber, int pageSize);

    Task<(List<RawUserApprovalSTPC>, int, List<AppUserRoles>)> GetReportDocumentApprovalAsync(string? keyword, string? fromDate, string? toDate, int pageNumber, int pageSize);

    Task<List<ReportDocumentSTPC>> GetListDocumentsByUser(string userId, string type, string? fromDate, string? toDate);
}

public class ReportRepository : IReportRepository
{
    private readonly QLTTrContext _context;

    public ReportRepository(QLTTrContext context)
    {
        _context = context;
    }

    public async Task<(List<ReportApprovalByUserSTPC>, int, List<AppUserRoles>)> GetReportApprovalByUserAsync(string? keyword, string? fromDate, string? toDate, int pageNumber, int pageSize)
    {
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@Keyword", keyword),
            new SqlParameter("@FromDate", fromDate),
            new SqlParameter("@ToDate", toDate),
            new SqlParameter("@PageNumber", pageNumber),
            new SqlParameter("@PageSize", pageSize),
        };

        var result = await _context.ExecuteStoredProcedureWithMultipleDatasetsAsync("ReportApprovalByUser", parameters);
        var reviewReports = result.Count > 0 ? DataHelper.ConvertToList<ReportApprovalByUserSTPC>(result[0].Cast<Dictionary<string, object>>().ToList()) : new List<ReportApprovalByUserSTPC>();
        var totalRecords = result.Count > 1 ? Convert.ToInt32(result[1].Cast<Dictionary<string, object>>().ToList()[0]["TotalRecords"]) : 0;
        var appUserRoles = result.Count > 2 ? DataHelper.ConvertToList<AppUserRoles>(result[2].Cast<Dictionary<string, object>>().ToList()) : new List<AppUserRoles>();

        return (reviewReports, totalRecords, appUserRoles);
    }

    public async Task<(List<RawUserApprovalSTPC>, int, List<AppUserRoles>)> GetReportDocumentApprovalAsync(string? keyword, string? fromDate, string? toDate, int pageNumber, int pageSize)
    {
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@Keyword", keyword),
            new SqlParameter("@FromDate", fromDate),
            new SqlParameter("@ToDate", toDate),
            new SqlParameter("@PageNumber", pageNumber),
            new SqlParameter("@PageSize", pageSize),
        };

        var result = await _context.ExecuteStoredProcedureWithMultipleDatasetsAsync("ReportDocumentApproval", parameters);
        var reviewReports = result.Count > 0 ? DataHelper.ConvertToList<RawUserApprovalSTPC>(result[0].Cast<Dictionary<string, object>>().ToList()) : new List<RawUserApprovalSTPC>();
        var totalRecords = result.Count > 1 ? Convert.ToInt32(result[1].Cast<Dictionary<string, object>>().ToList()[0]["TotalRecords"]) : 0;
        var appUserRoles = result.Count > 2 ? DataHelper.ConvertToList<AppUserRoles>(result[2].Cast<Dictionary<string, object>>().ToList()) : new List<AppUserRoles>();

        return (reviewReports, totalRecords, appUserRoles);
    }

    public async Task<(List<ReviewReportSTPC>, int, List<AppUserRoles>)> GetReviewReportAsync(string? keyword, string? fromDate, string? toDate, int? reviewResult, int? submitCount, int pageNumber, int pageSize, string? sortColumn, string? sortOrder)
    {
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@Keyword", keyword),
            new SqlParameter("@FromDate", fromDate),
            new SqlParameter("@ToDate", toDate),
            new SqlParameter("@ReviewResult", reviewResult),
            new SqlParameter("@SubmitCount", submitCount),
            new SqlParameter("@PageNumber", pageNumber),
            new SqlParameter("@PageSize", pageSize),
            new SqlParameter("@SortColumn", sortColumn),
            new SqlParameter("@SortOrder", sortOrder)
        };

        var result = await _context.ExecuteStoredProcedureWithMultipleDatasetsAsync("GetReviewReport", parameters);
        var reviewReports = result.Count > 0 ? DataHelper.ConvertToList<ReviewReportSTPC>(result[0].Cast<Dictionary<string, object>>().ToList()) : new List<ReviewReportSTPC>();
        var totalRecords = result.Count > 1 ? Convert.ToInt32(result[1].Cast<Dictionary<string, object>>().ToList()[0]["TotalRecords"]) : 0;
        var appUserRoles = result.Count > 2 ? DataHelper.ConvertToList<AppUserRoles>(result[2].Cast<Dictionary<string, object>>().ToList()) : new List<AppUserRoles>();

        return (reviewReports, totalRecords, appUserRoles);
    }

    public async Task<List<ReportDocumentSTPC>> GetListDocumentsByUser(string userId, string type, string? fromDate, string? toDate)
    {
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@UserId", userId),
            new SqlParameter("@Type", type),
            new SqlParameter("@FromDate", fromDate),
            new SqlParameter("@ToDate", toDate),
        };

        var result = await _context.ExecuteStoredProcedureWithMultipleDatasetsAsync("GetListDocumentsByUser", parameters);
        var reviewReports = result.Count > 0 ? DataHelper.ConvertToList<ReportDocumentSTPC>(result[0].Cast<Dictionary<string, object>>().ToList()) : new List<ReportDocumentSTPC>();

        return reviewReports;
    }
}
