using Core.QueryModels;
using Database.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Repositories.BaseRepository;
using Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ITblDepartmentRepository : IBaseRepository<TblDeparments>
    {
        Task<(List<AppUser>, List<AppUserRoles>)> GetUsersByDepartmentIdAsync(int departmentId);
    }

    public class TblDepartmentRepository : BaseRepository<TblDeparments>, ITblDepartmentRepository
    {
        private readonly QLTTrContext _context;

        public TblDepartmentRepository(QLTTrContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<AppUser>, List<AppUserRoles>)> GetUsersByDepartmentIdAsync(int departmentId)
        {
            var storedProcedureName = "GetUsersByDepartmentId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentId", departmentId)
            };

            return await _context.ExecuteStoredProcedureWithTwoDatasetsAsync<AppUser, AppUserRoles>(storedProcedureName, parameters);
        }
    }
}
