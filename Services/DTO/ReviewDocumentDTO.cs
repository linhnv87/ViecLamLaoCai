using Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO;

public class ReviewDocumentDTO
{
    public string? Keyword { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public int? ReviewResult { get; set; }
    public int? SubmitCount { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; }
    [Required]
    [Range(1, 1000)]
    public int PageSize { get; set; }

    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
}

[DataContract]
public class ReviewDocumentExportDTO
{
    [DataMember(Name = "#")]
    public int Index { get; set; }
    [DataMember(Name = "UserId")]
    public Guid UserId { get; set; }

    [DataMember(Name = "NoReview")]
    public int NoReview { get; set; }

    [DataMember(Name = "DisAgree")]
    public int DisAgree { get; set; }

    [DataMember(Name = "Agreed")]
    public int Agreed { get; set; }

    [DataMember(Name = "Other")]
    public int Other { get; set; }

    [DataMember(Name = "UserFullName")]
    public string UserFullName { get; set; }

    [DataMember(Name = "PhoneNumber")]
    public string PhoneNumber { get; set; }
}

public class ReivewDTO
{
    public Guid UserId { get; set; }
    public int NoReview { get; set; }
    public int DisAgree { get; set; }
    public int Agreed { get; set; }
    public int Other { get; set; }
    public string UserFullName { get; set; }
    public string PhoneNumber { get; set; }
    public List<RoleDTO>? Roles { get; set; }
}

public class ReportStatisticDTO
{
    public string? Keyword { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; }
    [Required]
    [Range(1, 1000)]
    public int PageSize { get; set; }
}

public class ReportApprovalByUserDTO
{
    public int Index { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public int TotalOfDocID { get; set; }
    public int NoReview { get; set; }
    public int DisAgreed { get; set; }
    public int Agreed { get; set; }
    public int Other { get; set; }
    public List<RoleDTO>? Roles { get; set; }
}

[DataContract]
public class ExportApprovalByUserDTO
{
    [DataMember(Name = "STT")]
    public int Index { get; set; }

    [DataMember(Name = @"Tên cán bộ")]
    public string UserFullName { get; set; }

    [DataMember(Name = @"Chức vụ")]
    public string RoleNames { get; set; }

    [DataMember(Name = "Đơn vị")]
    public string UnitName { get; set; }

    [DataMember(Name = "Phòng ban")]
    public string DepartmentName { get; set; }

    [DataMember(Name = "Tổng tờ trình")]
    public int TotalOfDocID { get; set; }

    [DataMember(Name = "Chưa cho ý kiến")]
    public int NoReview { get; set; }

    [DataMember(Name = "Không đồng ý")]
    public int DisAgreed { get; set; }

    [DataMember(Name = "Đồng ý")]
    public int Agreed { get; set; }

    [DataMember(Name = "Ý kiến khác")]
    public int Other { get; set; }
}

public class RawUserApprovalDTO
{
    public int Index { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public int TotalOfDocID { get; set; }
    public int Release { get; set; }
    public int NoRelease { get; set; }
    public List<RoleDTO>? Roles { get; set; }
}

[DataContract]
public class ExportRawUserApprovalDTO
{
    [DataMember(Name = "STT")]
    public int Index { get; set; }

    [DataMember(Name = @"Tên cán bộ")]
    public string UserFullName { get; set; }

    [DataMember(Name = @"Chức vụ")]
    public string RoleNames { get; set; }

    [DataMember(Name = "Đơn vị")]
    public string UnitName { get; set; }

    [DataMember(Name = "Phòng ban")]
    public string DepartmentName { get; set; }

    [DataMember(Name = "Tổng tờ trình")]
    public int TotalOfDocID { get; set; }

    [DataMember(Name = "Ban hành")]
    public int Release { get; set; }

    [DataMember(Name = "Không ban hành")]
    public int NoRelease { get; set; }
}
public class ReportDocumentDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Note { get; set; }
    public string StatusCode { get; set; }
    public Guid? AssigneeID { get; set; }
}
