using Database.Models;
using Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICfgWorkFlowGroupRepository : IBaseRepository<CfgWorkFlowGroup>
    {
    }
    public class CfgWorkFlowGrouplRepository : BaseRepository<CfgWorkFlowGroup>, ICfgWorkFlowGroupRepository
    {
        public CfgWorkFlowGrouplRepository(QLTTrContext context) : base(context)
        {

        }
    }
}
