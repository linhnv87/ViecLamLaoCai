using Database.Models;
using Microsoft.Data.SqlClient;
using Repositories.BaseRepository;
using Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories;

public interface ITblGroupRepository : IBaseRepository<TblGroups>
{
    Task<(List<AppUser>, List<AppUserRoles>)> GetUsersByGroupIdAsync(int groupId);
}

public class TblGroupRepository : BaseRepository<TblGroups>, ITblGroupRepository
{
    private readonly QLTTrContext _context;

    public TblGroupRepository(QLTTrContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(List<AppUser>, List<AppUserRoles>)> GetUsersByGroupIdAsync(int groupId)
    {
        var storedProcedureName = "GetUsersByGroupId";
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@GroupId", groupId)
        };

        return await _context.ExecuteStoredProcedureWithTwoDatasetsAsync<AppUser, AppUserRoles>(storedProcedureName, parameters);
    }
}
