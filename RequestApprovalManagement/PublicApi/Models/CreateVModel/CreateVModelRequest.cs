using System.ComponentModel.DataAnnotations;

namespace PublicApi.Models.CreateVModel;

public class CreateVModelRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
}
