using Aspose.Pdf.Operators;
using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace QuanLyToTrinh.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("Review")]
    public async Task<IActionResult> GetReviewReport([FromQuery] ReviewDocumentDTO payload)
    {
        try
        {
            var result = await _reportService.GetReviewReportAsync(payload);
            return Ok(new BaseResponseModel(result));
        }
        catch (Exception ex)
        {
            return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
        }
    }

    [HttpGet("Review/ExportBlob")]
    public async Task<IActionResult> ExportBlobReviewReport([FromQuery] ReviewDocumentDTO payload, [FromQuery] ExportFileDTO exportModel)
    {
        try
        {
            var content = await _reportService.ExportFileBlobAsync(payload, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
        catch (Exception ex)
        {
            return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
        }
    }

    [HttpGet("ApprovalStatistic")]
    public async Task<IActionResult> ReportApprovalStatistic([FromQuery] ReportStatisticDTO payload)
    {
        try
        {
            var result = await _reportService.GetReportApprovalStatisticAsync(payload);
            return Ok(new BaseResponseModel(result));
        }
        catch (Exception ex)
        {
            return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
        }
    }

    [HttpGet("ApprovalStatistic/Export")]
    public async Task<IActionResult> ExportApprovalStatistic([FromQuery] ReportStatisticDTO payload, [FromQuery] ExportFileDTO exportModel)
    {
        try
        {
            var content = await _reportService.ExportApprovalStatisticAsync(payload, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
        catch (Exception ex)
        {
            return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
        }
    }

    [HttpGet("DocumentApproval")]
    public async Task<IActionResult> ReportDocumentApproval([FromQuery] ReportStatisticDTO payload)
    {
        try
        {
            var result = await _reportService.ReportDocumentApprovalAsync(payload);
            return Ok(new BaseResponseModel(result));
        }
        catch (Exception ex)
        {
            return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
        }
    }

    [HttpGet("DocumentApproval/Export")]
    public async Task<IActionResult> ExportDocumentApproval([FromQuery] ReportStatisticDTO payload, [FromQuery] ExportFileDTO exportModel)
    {
        try
        {
            var content = await _reportService.ExportDocumentApprovalAsync(payload, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
        catch (Exception ex)
        {
            return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
        }
    }

    [HttpGet("{userId}/Documents/{type}")]
    public async Task<IActionResult> ListDocumentsByUser(
     [FromRoute] string userId,
     [FromRoute] string type,
     [FromQuery] string? fromDate = null,
     [FromQuery] string? toDate = null)
    {
        try
        {
            if (type == "to-trinh" || type == "xin-y-kien")
            {
                var result = await _reportService.GetListDocuments(userId, type, fromDate, toDate);
                return Ok(new BaseResponseModel(result));
            }

            return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, "type: to-trinh | xin-y-kien"));
        }
        catch (Exception ex)
        {
            return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
        }
    }

}
