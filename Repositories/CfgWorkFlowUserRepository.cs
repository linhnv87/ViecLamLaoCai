using Database.Models;
using Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICfgWorkFlowUser : IBaseRepository<CfgWorkFlowUser>
    {
    }
    public class CfgWorkFlowUserRepository : BaseRepository<CfgWorkFlowUser>, ICfgWorkFlowUser
    {
        public CfgWorkFlowUserRepository(QLTTrContext context) : base(context)
        {

        }
    }
}
