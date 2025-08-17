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
    public interface IFieldRepository : IBaseRepository<TblField>
    {

    }
    public class FieldRepository : BaseRepository<TblField>, IFieldRepository
    {
        public FieldRepository(QLTTrContext context) : base(context)
        {

        }
    }
}
