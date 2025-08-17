using AutoMapper;
using Azure.Core;
using Core.Helpers;
using Database.Models;
using Database.STPCModels;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Repositories;
using Repositories.BaseRepository;
using Services.DTO;
using Services.Helper;
using System.IO;

namespace Services;

public interface IReportService
{
    Task<PaginationData<ReivewDTO>> GetReviewReportAsync(ReviewDocumentDTO payload);
    Task<string> ExportFileAsync(ReviewDocumentDTO payload, ExportFileDTO exportModel);
    Task<ExportStream> ExportFileBlobAsync(ReviewDocumentDTO payload, ExportFileDTO exportModel);
    Task<PaginationData<ReportApprovalByUserDTO>> GetReportApprovalStatisticAsync(ReportStatisticDTO payload);
    Task<ExportStream> ExportApprovalStatisticAsync(ReportStatisticDTO payload, ExportFileDTO exportModel);
    Task<PaginationData<RawUserApprovalDTO>> ReportDocumentApprovalAsync(ReportStatisticDTO payload);
    Task<ExportStream> ExportDocumentApprovalAsync(ReportStatisticDTO payload, ExportFileDTO exportModel);
    Task<IEnumerable<ReportDocumentDTO>> GetListDocuments(string userId, string type, string? fromDate, string? toDate);
}

public class ReportService : IReportService
{
    private readonly ILogger<ReportService> _logger;
    private readonly IReportRepository _reportRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;

    public ReportService(ILogger<ReportService> logger, IReportRepository reportRepository, IMapper mapper, IWebHostEnvironment env)
    {
        _logger = logger;
        _reportRepository = reportRepository;
        _mapper = mapper;
        _env = env;
    }

    public async Task<PaginationData<ReivewDTO>> GetReviewReportAsync(ReviewDocumentDTO payload)
    {
        var (data, totalCount, roles) = await _reportRepository
            .GetReviewReportAsync(
            payload.Keyword,
            payload.FromDate,
            payload.ToDate,
            payload.ReviewResult,
            payload.SubmitCount,
            payload.PageNumber,
            payload.PageSize,
            payload.SortColumn,
            payload.SortOrder);

        var reviewsDto = _mapper.Map<IEnumerable<ReviewReportSTPC>, IEnumerable<ReivewDTO>>(data).ToList();
        foreach (var review in reviewsDto)
        {
            var userRoles = roles.Where(x => x.UserId == review.UserId);
            review.Roles = _mapper.Map<List<RoleDTO>>(userRoles).ToList();
        }

        var result = PaginationData<ReivewDTO>.Create(reviewsDto, payload.PageNumber, payload.PageSize, totalCount);

        return result;
    }

    public async Task<string> ExportFileAsync(ReviewDocumentDTO payload, ExportFileDTO exportModel)
    {
        var (data, _, _) = await _reportRepository.GetReviewReportAsync(
            payload.Keyword,
            payload.FromDate,
            payload.ToDate,
            payload.ReviewResult,
            payload.SubmitCount,
            1,
            1000,
            payload.SortColumn,
            payload.SortOrder);
        var formatedData = _mapper.Map<IEnumerable<ReviewReportSTPC>, IEnumerable<ReviewDocumentExportDTO>>(data);
        var exportFileInfoDTO = new ExportFileInfoDTO
        {
            FileName = exportModel.FileName,
            SheetName = exportModel.SheetName,
            Type = exportModel.Type,
            Title = "Review Report",
            Description = "This is the report of all reviews"
        };
        var exportData = ImportExportHelper<ReviewDocumentExportDTO>.ExportFile(exportFileInfoDTO, formatedData);
        var filePath = Path.Combine(_env.WebRootPath, "Files", "Document_Attachments", $"{DateTime.Today:yyyy-MM-dd}_{exportData.FileName}");
        var linkToFile = Path.Combine("/Files", "Document_Attachments", $"{DateTime.Today:yyyy-MM-dd}_{exportData.FileName}");

        using (var memoryStream = exportData.Stream)
        {
            File.WriteAllBytes(filePath, memoryStream.ToArray());
        }

        return linkToFile ?? "";
    }

    public async Task<ExportStream> ExportFileBlobAsync(ReviewDocumentDTO payload, ExportFileDTO exportModel)
    {
        var (data, _, _) = await _reportRepository.GetReviewReportAsync(
            payload.Keyword,
            payload.FromDate,
            payload.ToDate,
            payload.ReviewResult,
            payload.SubmitCount,
            1,
            1000,
            payload.SortColumn,
            payload.SortOrder);
        var formatedData = _mapper.Map<IEnumerable<ReviewReportSTPC>, IEnumerable<ReviewDocumentExportDTO>>(data);
        var exportFileInfoDTO = new ExportFileInfoDTO
        {
            FileName = exportModel.FileName,
            SheetName = exportModel.SheetName,
            Type = exportModel.Type,
            Title = "Review Report",
            Description = "This is the report of all reviews"
        };
        var exportData = ImportExportHelper<ReviewDocumentExportDTO>.ExportFile(exportFileInfoDTO, formatedData);

        return await Task.FromResult(exportData);
    }

    public async Task<PaginationData<ReportApprovalByUserDTO>> GetReportApprovalStatisticAsync(ReportStatisticDTO payload)
    {
        var (data, totalCount, roles) = await _reportRepository.GetReportApprovalByUserAsync(payload.Keyword, payload.FromDate, payload.ToDate, payload.PageNumber, payload.PageSize);
        var reviewsDto = _mapper.Map<IEnumerable<ReportApprovalByUserSTPC>, IEnumerable<ReportApprovalByUserDTO>>(data).ToList();
        foreach (var review in reviewsDto)
        {
            var userRoles = roles.Where(x => x.UserId == review.UserId);
            review.Roles = _mapper.Map<List<RoleDTO>>(userRoles).ToList();
        }

        var result = PaginationData<ReportApprovalByUserDTO>.Create(reviewsDto, payload.PageNumber, payload.PageSize, totalCount);
        return result;
    }

    public async Task<ExportStream> ExportApprovalStatisticAsync(ReportStatisticDTO payload, ExportFileDTO exportModel)
    {
        var data = await GetReportApprovalStatisticAsync(payload);
        var formatedData = _mapper.Map<IEnumerable<ReportApprovalByUserDTO>, IEnumerable<ExportApprovalByUserDTO>>(data.Data);
        var exportFileInfoDTO = new ExportFileInfoDTO
        {
            FileName = exportModel.FileName,
            SheetName = exportModel.SheetName,
            Type = exportModel.Type,
            Title = "BIỂU THỐNG KÊ Ý KIẾN",
            Description = DateFormatHelper.ConvertDateRange(payload.FromDate, payload.ToDate)
        };
        var exportData = ImportExportHelper<ExportApprovalByUserDTO>.ExportFile(exportFileInfoDTO, formatedData);

        return await Task.FromResult(exportData);
    }

    public async Task<PaginationData<RawUserApprovalDTO>> ReportDocumentApprovalAsync(ReportStatisticDTO payload)
    {
        var (data, totalCount, roles) = await _reportRepository.GetReportDocumentApprovalAsync(payload.Keyword, payload.FromDate, payload.ToDate, payload.PageNumber, payload.PageSize);
        var reviewsDto = _mapper.Map<IEnumerable<RawUserApprovalSTPC>, IEnumerable<RawUserApprovalDTO>>(data).ToList();
        foreach (var review in reviewsDto)
        {
            var userRoles = roles.Where(x => x.UserId == review.UserId);
            review.Roles = _mapper.Map<List<RoleDTO>>(userRoles).ToList();
        }

        var result = PaginationData<RawUserApprovalDTO>.Create(reviewsDto, payload.PageNumber, payload.PageSize, totalCount);
        return result;
    }

    public async Task<ExportStream> ExportDocumentApprovalAsync(ReportStatisticDTO payload, ExportFileDTO exportModel)
    {
        var data = await ReportDocumentApprovalAsync(payload);
        var formatedData = _mapper.Map<IEnumerable<RawUserApprovalDTO>, IEnumerable<ExportRawUserApprovalDTO>>(data.Data);
        var exportFileInfoDTO = new ExportFileInfoDTO
        {
            FileName = exportModel.FileName,
            SheetName = exportModel.SheetName,
            Type = exportModel.Type,
            Title = "BIỂU THỐNG KÊ TỜ TRÌNH",
            Description = DateFormatHelper.ConvertDateRange(payload.FromDate, payload.ToDate)
        };
        var exportData = ImportExportHelper<ExportRawUserApprovalDTO>.ExportFile(exportFileInfoDTO, formatedData);

        return await Task.FromResult(exportData);
    }

    public async Task<IEnumerable<ReportDocumentDTO>> GetListDocuments(string userId, string type, string? fromDate = null, string? toDate = null)
    {
        var data = await _reportRepository.GetListDocumentsByUser(userId, type, fromDate, toDate);
        var documentDto = _mapper.Map<IEnumerable<ReportDocumentSTPC>, IEnumerable<ReportDocumentDTO>>(data);
        return documentDto;
    }
}
