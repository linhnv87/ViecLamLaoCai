using Database.Models;
using Database;
using Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ITblFieldRepository : IBaseRepository<TblField>
    {

    }
    public class TblFieldRepository : BaseRepository<TblField>, ITblFieldRepository
    {
        public TblFieldRepository(QLTTrContext context) : base(context)
        {

        }
    }
}
