using Application;
using Application.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static Authentication.Models.CommonModels;

namespace Authentication.Repositories
{

    public interface ICommonRepository
    {
        Task<List<Common_Department_Result>> GetDepartmentsAsync(Common_Department_Criteria criteria);
        Task<List<Common_Position_Result>> GetPositionsAsync(Common_Position_Criteria criteria);
        Task<List<Common_Role_Result>> GetRolesAsync(Common_Role_Criteria criteria);

        Task<List<Common_SystemConfigs_Result>> SystemConfigs(Common_SystemConfigs_Criteria criteria);


    }
    public class CommonRepository : ICommonRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly SystemDbContext _systemDb;
        private readonly IMapper _mapper;

        public CommonRepository(ApplicationDbContext db, IMapper mapper, SystemDbContext systemDb)
        {
            _db = db;
            _mapper = mapper;
            _systemDb = systemDb;
        }

        public async Task<List<Common_SystemConfigs_Result>> SystemConfigs(Common_SystemConfigs_Criteria criteria)
        {



            var result = await _systemDb.TsSystemConfigs
                .Where(x => criteria.ConfigCodes.Contains(x.ConfigCode))
                .Select(x => new Common_SystemConfigs_Result
                {
                    ConfigCode = x.ConfigCode,
                    ConfigDesc = x.ConfigDesc,
                    ValueVarchar = x.ValueVarchar,
                    ValueVarchar2 = x.ValueVarchar2,
                    ValueVarchar3 = x.ValueVarchar3,
                    ValueInt = x.ValueInt
                })
                .ToListAsync();

            return result;
        }


        public async Task<List<Common_Department_Result>> GetDepartmentsAsync(Common_Department_Criteria criteria)
        {
            var query = _db.Set<tb_Department>().AsQueryable();

            // ยังไม่ใช้เงื่อนไขจาก criteria (future use)

            return await query
                .OrderBy(d => d.DepartmentCode)
                .Select(d => new Common_Department_Result
                {
                    Id = d.Id,
                    DepartmentCode = d.DepartmentCode,
                    DepartmentName = d.DepartmentName
                })
                .ToListAsync();
        }


        public async Task<List<Common_Position_Result>> GetPositionsAsync(Common_Position_Criteria criteria)
        {
            var query = _db.Set<tb_Position>().AsQueryable();

            // ยังไม่ใช้เงื่อนไขจาก criteria (future use)

            return await query
                .Where(p => p.PositionCode != "00")
                .OrderBy(p => p.PositionCode)
                .Select(p => new Common_Position_Result
                {
                    Id = p.Id,
                    PositionCode = p.PositionCode,
                    PositionName = p.PositionName
                })
                .ToListAsync();
        }

        public async Task<List<Common_Role_Result>> GetRolesAsync(Common_Role_Criteria criteria)
        {
            var query = _db.Set<ApplicationRole>().AsQueryable();

            // ยังไม่ใช้เงื่อนไขจาก criteria (future use)

            return await query
                .OrderBy(r => r.Name)
                .Select(r => new Common_Role_Result
                {
                    Id = r.Id,
                    Name = r.Name,
                    NormalizedName = r.NormalizedName,
                    Description = r.Description,
                    IsActive = r.IsActive
                })
                .ToListAsync();
        }

    }
}
