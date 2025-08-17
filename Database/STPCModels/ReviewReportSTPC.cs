namespace Database.STPCModels;

public class ReviewReportSTPC
{
    public long Index { get; set; }
    public Guid UserId { get; set; }
    public int NoReview { get; set; }
    public int DisAgree { get; set; }
    public int Agreed { get; set; }
    public int Other { get; set; }
    public string UserFullName { get; set; }
    public string PhoneNumber { get; set; }
}

public class ReportApprovalByUserSTPC
{
    public long Index { get; set; }
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
}

public class RawUserApprovalSTPC
{
    public long Index { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public int TotalOfDocID { get; set; }
    public int Release { get; set; }
    public int NoRelease { get; set; }
}

public class ReportDocumentSTPC
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Note { get; set; }
    public string StatusCode { get; set; }
    public Guid? AssigneeID { get; set; }
}
