using Core.QueryModels;
using Database.Models;
using Database.STPCModels;
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
    public interface ICfgWorkFlowRepository : IBaseRepository<CfgWorkFlow>
    {
        Task<(List<CfgWorkFlow> workFlows, List<CfgUsersWorkFlowQueryModel> users, List<CfgRolesModel> roles)> GetWrokFlowByUserAsync(string userId, int statusId, string state);
    }
    public class CfgWorkFlowRepository : BaseRepository<CfgWorkFlow>, ICfgWorkFlowRepository
    {
        private readonly QLTTrContext _context;

        public CfgWorkFlowRepository(QLTTrContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<CfgWorkFlow> workFlows, List<CfgUsersWorkFlowQueryModel> users, List<CfgRolesModel> roles)> GetWrokFlowByUserAsync(string userId, int statusId, string state)
        {
            var storedProcedureName = "GetUsersByWorkFlow";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@StatusId", statusId),
                new SqlParameter("@State", state)
            };

            var resultSets = await _context.ExecuteStoredProcedureWithMultipleDatasetsAsync(storedProcedureName, parameters);

            var workFlows = resultSets.Count > 0 ? DataHelper.ConvertToList<CfgWorkFlow>(resultSets[0].Cast<Dictionary<string, object>>().ToList()) : new List<CfgWorkFlow>();
            var users = resultSets.Count > 1 ? DataHelper.ConvertToList<CfgUsersWorkFlowQueryModel>(resultSets[1].Cast<Dictionary<string, object>>().ToList()) : new List<CfgUsersWorkFlowQueryModel>();
            var roles = resultSets.Count > 2 ? DataHelper.ConvertToList<CfgRolesModel>(resultSets[2].Cast<Dictionary<string, object>>().ToList()) : new List<CfgRolesModel>();

            return (workFlows, users, roles);
        }
    }
}
