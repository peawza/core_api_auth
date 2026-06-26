

using Application;
using ApplicationDB.Models.System;
using Authentication.Repositories;
using static Authentication.Models.UMS030.UMS030;

namespace Authentication.Services
{
    public partial interface IUMS030Service
    {
        Task<IEnumerable<GroupPermissionDataView>> ListByUserGroup(string userGroupId);
        Task<IEnumerable<ts_ScreenFunction>> ListScreenFunctions();
        Task<UMS030_UpdatePermission_Result> UpdatePermission(UMS030_UpdatePermission_Criteria permissions);
        Task<List<UMS030_GetRoles_Result>>? GetRoles(UMS030_GetRoles_Criteria criteria);
        Task<List<UMS030_GetModule_Result>>? GetModule(UMS030_GetModule_Criteria criteria);
    }
    public class UMS030Service : IUMS030Service
    {
        private readonly IUMS030Repository _repository;
        private readonly ApplicationDbContext _db;

        public UMS030Service(IUMS030Repository repository, ApplicationDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<IEnumerable<GroupPermissionDataView>> ListByUserGroup(string userGroupId)
        {
            return await _repository.ListByUserGroup(userGroupId);
        }

        public async Task<IEnumerable<ts_ScreenFunction>> ListScreenFunctions()
        {
            return await _repository.ListScreenFunctions();
        }

        public async Task<UMS030_UpdatePermission_Result> UpdatePermission(UMS030_UpdatePermission_Criteria permissions)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var result = await _repository.UpdatePermission(permissions);

                if (result.StatusCode == "SUCCESS")
                    await transaction.CommitAsync();
                else
                    await transaction.RollbackAsync();

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new UMS030_UpdatePermission_Result
                {
                    StatusCode = "ERROR",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "SERVICE_CREATE_EXCEPTION",
                    MessageName = ex.Message
                };
            }
        }
        public async Task<List<UMS030_GetRoles_Result>>? GetRoles(UMS030_GetRoles_Criteria criteria)
        {
            try
            {
                return await _repository.GetRoles(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<UMS030_GetModule_Result>>? GetModule(UMS030_GetModule_Criteria criteria)
        {
            try
            {
                return await _repository.GetModule(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
