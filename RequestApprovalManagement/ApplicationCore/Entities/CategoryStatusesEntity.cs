using ApplicationCore.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities;

[Table("CategoryStatuses")]
public class CategoryStatusesEntity : IAggregateRoot
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
