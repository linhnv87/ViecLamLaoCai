using Database.Models;
using Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IUnitRepository : IBaseRepository<TblUnit>
    {

    }
    public class UnitRepository : BaseRepository<TblUnit>, IUnitRepository
    {
        public UnitRepository(QLTTrContext context) : base(context)
        {

        }
    }
}
