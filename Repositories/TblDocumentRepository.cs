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
    public interface ITblDocumentRepository : IBaseRepository<TblDocument>
    {

    }
    public class TblDocumentRepository : BaseRepository<TblDocument>, ITblDocumentRepository
    {
        public TblDocumentRepository(QLTTrContext context) : base(context)
        {

        }
    }

}
