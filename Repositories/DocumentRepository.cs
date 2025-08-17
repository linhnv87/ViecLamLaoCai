using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Database;
using Database.Models;
using Microsoft.Data.SqlClient;
using Repositories.BaseRepository;
using Repositories.Extensions;

namespace Repositories
{
    public interface IDocumentRepository : IBaseRepository<TblDocument>
    {
        Task<int> GetDocumentCountByMonth(int month);
        Task<int> GetDocumentCountByField(int field);
        Task<int> CountDocumentByStatus(string status);
        Task<int> RetrieveDocumentAsync(int documentId, string note, string comment, Guid currentUserId);

    }
    public class DocumentRepository : BaseRepository<TblDocument>, IDocumentRepository
    {
        private readonly QLTTrContext _context;
        public DocumentRepository(QLTTrContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> GetDocumentCountByMonth(int month)
        {
            // Assuming TblDocument has a DateTime field named 'DateCreated' or similar to check against
            DateTime startOfMonth = new DateTime(DateTime.Now.Year, month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            int count = _context.Set<TblDocument>().Where(d => d.StatusCode != AppDocumentStatuses.DU_THAO && d.Created >= startOfMonth && d.Created <= endOfMonth).Count();

            return count;
        }
        public async Task<int> GetDocumentCountByField(int field)
        {
            return _context.Set<TblDocument>()
                .Where(d => d.StatusCode != AppDocumentStatuses.DU_THAO && d.FieldId == field)
                .Count();
        }
        public async Task<int> CountDocumentByStatus(string status)
        {
            return _context.Set<TblDocument>()
                .Where(d => d.StatusCode != AppDocumentStatuses.DU_THAO && d.StatusCode == status)
                .Count();
        }

        public async Task<int> RetrieveDocumentAsync(int documentId, string note, string comment, Guid currentUserId)
        {
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@DocumentId", documentId),
                new SqlParameter("@Note", note),
                new SqlParameter("@Comment", comment),
                new SqlParameter("@CurrentUserId", currentUserId)
            };

            return await _context.ExecuteStoredProcedureNonQueryAsync("RetrieveDocument", sqlParams);
        }
    }

}
