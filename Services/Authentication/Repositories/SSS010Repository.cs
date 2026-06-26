using Application;
using Application.Models;
using Microsoft.EntityFrameworkCore;
using static Authentication.Models.SSS010.SSS010;

namespace Authentication.Repositories
{
    public partial interface ISSS010Repository
    {


        Task<List<SSS010_GetGroupPermissionUser_Result>> GetGroupPermissionUser(SSS010_GetGroupPermissionUser_Criteria criteria);
        Task<string?> WhereAllData(SSS010_WhereAllData_Criteria criteria);


    }
    public class SSS010Repository : ISSS010Repository
    {


        private readonly ApplicationDbContext _db;
        private readonly SystemDbContext _systemDb;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SSS010Repository(ApplicationDbContext db, SystemDbContext systemDb, IHttpContextAccessor httpContextAccessor)
        {

            _db = db;
            _systemDb = systemDb;

            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<List<SSS010_GetGroupPermissionUser_Result>> GetGroupPermissionUser(SSS010_GetGroupPermissionUser_Criteria criteria)
        {
            var query = from gp in _db.Set<tb_GroupPermission>().AsNoTracking()
                        join r in _db.Roles on gp.GroupId equals r.Id
                        join ur in _db.UserRoles on r.Id equals ur.RoleId
                        where ur.UserId == criteria.UserId
                        orderby gp.ScreenId
                        select new SSS010_GetGroupPermissionUser_Result
                        {
                            ScreenID = gp.ScreenId,
                            FunctionCode = gp.FunctionCode
                        };

            return await query.ToListAsync();
        }


        public async Task<string?> WhereAllData(SSS010_WhereAllData_Criteria criteria)
        {
            var WhereAllData = await _systemDb.TsSystemConfigs
                                .AsNoTracking()
                                .Where(x => x.ConfigCode == "WhereAllData")
                                .Select(x => x.ValueVarchar)
                                .FirstOrDefaultAsync();

            return WhereAllData;
        }

    }
}
