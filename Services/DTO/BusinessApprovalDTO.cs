using System;
using System.Collections.Generic;

namespace Services.DTO
{
    public class BusinessApprovalListResponseDTO
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string TaxCode { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string BusinessType { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; } // pending, reviewing, approved, rejected
        public string LogoUrl { get; set; }
        public List<BusinessDocumentDTO> Documents { get; set; } = new List<BusinessDocumentDTO>();
        public string Notes { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class BusinessDocumentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public DateTime UploadDate { get; set; }
        public bool Verified { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }
    }

    public class BusinessApprovalStatsDTO
    {
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Reviewing { get; set; }
        public int TodaySubmissions { get; set; }
        public int TotalSubmissions { get; set; }
    }

    public class BusinessApprovalFilterDTO
    {
        public string Status { get; set; } = "all"; // all, pending, reviewing, approved, rejected
        public string BusinessType { get; set; } = "all"; // all, specific business type
        public string SearchKeyword { get; set; } = "";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "registrationDate"; // registrationDate, businessName, status
        public string SortOrder { get; set; } = "desc"; // asc, desc
    }

    public class PaginatedBusinessApprovalResponseDTO
    {
        public List<BusinessApprovalListResponseDTO> Data { get; set; } = new List<BusinessApprovalListResponseDTO>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class ApproveBusinessRequestDTO
    {
        public int BusinessId { get; set; }
        public string ApprovedBy { get; set; }
        public string Notes { get; set; } = "";
    }

    public class RejectBusinessRequestDTO
    {
        public int BusinessId { get; set; }
        public string RejectedBy { get; set; }
        public string Reason { get; set; }
    }

    public class SetReviewingRequestDTO
    {
        public int BusinessId { get; set; }
        public string ReviewedBy { get; set; }
        public string Notes { get; set; } = "";
    }

    public class BusinessApprovalDetailResponseDTO : BusinessApprovalListResponseDTO
    {
        public string Website { get; set; }
        public string CompanySize { get; set; }
        public string Industry { get; set; }
        public string Description { get; set; }
        public string Position { get; set; }
        public List<BusinessApprovalHistoryDTO> ApprovalHistory { get; set; } = new List<BusinessApprovalHistoryDTO>();
    }

    public class BusinessApprovalHistoryDTO
    {
        public int Id { get; set; }
        public string Action { get; set; } // submitted, approved, rejected, set_reviewing
        public string ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public string Notes { get; set; }
        public string PreviousStatus { get; set; }
        public string NewStatus { get; set; }
    }

    public class BulkApproveRequestDTO
    {
        public List<int> BusinessIds { get; set; } = new List<int>();
        public string ApprovedBy { get; set; }
    }

    public class BulkRejectRequestDTO
    {
        public List<int> BusinessIds { get; set; } = new List<int>();
        public string RejectedBy { get; set; }
        public string Reason { get; set; }
    }

    public class ExportBusinessApprovalsRequestDTO
    {
        public BusinessApprovalFilterDTO Filter { get; set; } = new BusinessApprovalFilterDTO();
        public string Format { get; set; } = "excel"; // excel, pdf
    }
}
