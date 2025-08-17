using Core.QueryModels;
using Database.Models;
using Repositories.BaseRepository;
using Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IStatusesRepository : IBaseRepository<TblStatuses>
    {
        Task<List<StatutusByUserQueryModel>> GetStatusByUser(Guid userId);
    }

    public class StatusesRepository : BaseRepository<TblStatuses>, IStatusesRepository
    {
        private readonly QLTTrContext _context;

        public StatusesRepository(QLTTrContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<StatutusByUserQueryModel>> GetStatusByUser(Guid userId)
        {
            string storedProcedure = "GetTotalOfReviews";
            var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@UserId", userId)
            };

            return await _context.ExecuteStoredProcedureAsync<StatutusByUserQueryModel>(storedProcedure, parameters);
        }
    }
}
