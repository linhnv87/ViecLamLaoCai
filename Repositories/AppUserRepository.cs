using Core;
using Database;
using Database.Models;
using Database.STPCModels;
using Microsoft.Data.SqlClient;
using Repositories.BaseRepository;
using Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Repositories
{
    public interface IUserRepository : IBaseRepository<AppUser>
    {
        Task<List<AppUser>> GetUsersByRoleAsync(string roleName);
        Task<List<AppUser>> GetUserWithRolesAsync();
        Task<(List<AppUser>, List<AppUserRoles>)> GetAllUserWithRolesAsync();
    }
    public class UserRepository : BaseRepository<AppUser>, IUserRepository
    {
        private readonly QLTTrContext _context;
        public UserRepository(QLTTrContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<AppUser>> GetUsersByRoleAsync(string roleName)
        {
            var storedProcedureName = "GetUsersByRole";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@RoleName", roleName)
            };
            return await _context.ExecuteStoredProcedureAsync<AppUser>(storedProcedureName, parameters);
        }
        public async Task<List<AppUser>> GetUserWithRolesAsync()
        {
            var banChapHanhRole = AppRoleNames.BAN_CHAP_HANH;
            var banThuongVuRole = AppRoleNames.BAN_THUONG_VU;

            var storedProcedureName = "GetUsersWithRoles";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BanChapHanhRole", banChapHanhRole),
                new SqlParameter("@BanThuongVuRole", banThuongVuRole)
            };
            return await _context.ExecuteStoredProcedureAsync<AppUser>(storedProcedureName, parameters);
        }

        public async Task<(List<AppUser>, List<AppUserRoles>)> GetAllUserWithRolesAsync()
        {
            var storedProcedureName = "GetAllAppUsers";
            var resultSets = await _context.ExecuteStoredProcedureWithMultipleDatasetsAsync(storedProcedureName);
            var users = resultSets.Count > 0 ? DataHelper.ConvertToList<AppUser>(resultSets[0].Cast<Dictionary<string, object>>().ToList()) : new List<AppUser>();
            var roles = resultSets.Count > 1 ? DataHelper.ConvertToList<AppUserRoles>(resultSets[1].Cast<Dictionary<string, object>>().ToList()) : new List<AppUserRoles>();

            return (users, roles);
        }
    }
    public interface IRoleRepository : IBaseRepository<AppRole>
    {
    }
    public class RoleRepository : BaseRepository<AppRole>, IRoleRepository
    {
        public RoleRepository(QLTTrContext context) : base(context)
        {

        }
    }

    public interface IUserInRoleRepository : IBaseRepository<AppUserInRole>
    {
    }
    public class UserInRoleRepository : BaseRepository<AppUserInRole>, IUserInRoleRepository
    {
        public UserInRoleRepository(QLTTrContext context) : base(context)
        {

        }
    }

    //public interface IMembershipRepository : IBaseRepository<AspnetMembership>
    //{
    //}
    //public class MembershipRepository : BaseRepository<AspnetMembership>, IMembershipRepository
    //{
    //    public MembershipRepository(QLTTrContext context) : base(context)
    //    {

    //    }
    //}


}
