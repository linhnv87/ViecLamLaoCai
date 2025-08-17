using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string? NotificationContent { get; set; }
        public string? NotificationLink { get; set; }
        public int? Type { get; set; }
        public Guid? ForUserId { get; set; }
        public bool Watched { get; set; } = false;
    }
}
