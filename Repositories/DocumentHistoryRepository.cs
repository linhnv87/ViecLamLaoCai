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
    public interface IDocumentHistoryRepository : IBaseRepository<TblDocumentHistory>
    {

    }
    public class DocumentHistoryRepository : BaseRepository<TblDocumentHistory>, IDocumentHistoryRepository
    {
        public DocumentHistoryRepository(QLTTrContext context) : base(context)
        {

        }
    }
}
