using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities;

public partial class TblSMSLog
{
    [Key]
    public int Id { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsSucceeded { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
}
