using Core.CustomValidationAttribute;
using System.ComponentModel.DataAnnotations;

namespace Services.DTO;

public class ExportFileDTO
{
    [Required]
    public string FileName { get; set; }

    [Required]
    public string SheetName { get; set; }

    [Required]
    [StringInList("EXCEL", "PDF")]
    public string Type { get; set; }
}

public class ExportFileInfoDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string FileName { get; set; }
    public string SheetName { get; set; }
    public string Type { get; set; }
}
