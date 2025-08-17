using ApplicationCore.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities;
[Table("CategoryDocumentTypes")]

public class CategoryDocumentTypesEntity : IAggregateRoot
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
