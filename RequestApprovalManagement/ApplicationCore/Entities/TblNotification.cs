using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities;

public class TblNotification
{
    [Key]
    public int Id { get; set; }
    public string? NotificationContent { get; set; }
    public string? NotificationLink { get; set; }
    public int? Type { get; set; }
    public Guid? ForUserId { get; set; }
    public bool Watched { get; set; } = false;

}
