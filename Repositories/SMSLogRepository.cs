using Database.Models;
using Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ISMSLogRepository : IBaseRepository<TblSMSLog>
    {

    }
    public class SMSLogRepository : BaseRepository<TblSMSLog>, ISMSLogRepository
    {
        public SMSLogRepository(QLTTrContext context) : base(context)
        {

        }
    }
}
