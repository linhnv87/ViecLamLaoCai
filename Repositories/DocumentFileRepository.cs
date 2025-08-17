using Database.Models;
using Database;
using Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface IDocumentFileRepository : IBaseRepository<TblDocumentFile>
    {

    }
    public class DocumentFileRepository : BaseRepository<TblDocumentFile>, IDocumentFileRepository
    {
        private readonly QLTTrContext _context;
        public DocumentFileRepository(QLTTrContext context) : base(context)
        {
            _context = context;
        }
    }
}
