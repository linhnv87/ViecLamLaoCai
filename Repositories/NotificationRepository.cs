using Database.Models;
using Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{    
    public interface INotificationRepository : IBaseRepository<TblNotification>
    {

    }
    public class NotificationRepository : BaseRepository<TblNotification>, INotificationRepository
    {
        public NotificationRepository(QLTTrContext context) : base(context)
        {

        }
    }
}
