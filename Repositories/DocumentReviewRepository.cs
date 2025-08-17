using Database.Models;
using Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IDocumentReviewRepository : IBaseRepository<TblDocumentReview>
    {
    }
    public class DocumentReviewRepository : BaseRepository<TblDocumentReview>, IDocumentReviewRepository
    {
        private readonly QLTTrContext _context;
        public DocumentReviewRepository(QLTTrContext context) : base(context)
        {
            _context = context;
        }
    }
}