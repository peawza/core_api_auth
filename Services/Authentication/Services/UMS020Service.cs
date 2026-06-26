

using Application;
using Authentication.Repositories;
using static Authentication.Models.UMS020.UMS020;

namespace Authentication.Services
{
    public partial interface IUMS020Service
    {
        Task<List<MS020_SearchRole_Result>> UMS020_SearchRole(UMS020_SearchRole_Criteria criteria);
        Task<UMS020_AddRole_Result> UMS020_AddRole(UMS020_AddRole_Criteria criteria);
        Task<UMS020_UpdateRole_Result> UMS020_UpdateRole(UMS020_UpdateRole_Criteria criteria);
        Task<UMS020_DeleteRole_Result> DeleteRoleAsync(UMS020_DeleteRole_Criteria criteria);
        Task<UMS020_UserRoleMapping_Result> AddUsersToRole(UMS020_UserRoleMapping_Criteria data);
        Task<UMS020_GetUserRoleMapping_Result> GetUserRoleMapping(UMS020_GetUserRoleMapping_Criteria criteria);
    }
    public class UMS020Service : IUMS020Service
    {
        private readonly IUMS020Repository _repository;
        private readonly ApplicationDbContext _db;

        public UMS020Service(IUMS020Repository repository, ApplicationDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<List<MS020_SearchRole_Result>> UMS020_SearchRole(UMS020_SearchRole_Criteria criteria)
        {
            try
            {
                return await _repository.UMS020_SearchRole(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UMS020_AddRole_Result> UMS020_AddRole(UMS020_AddRole_Criteria criteria)
        {
            try
            {
                return await _repository.UMS020_AddRole(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UMS020_UpdateRole_Result> UMS020_UpdateRole(UMS020_UpdateRole_Criteria criteria)
        {
            try
            {
                return await _repository.UMS020_UpdateRole(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UMS020_DeleteRole_Result> DeleteRoleAsync(UMS020_DeleteRole_Criteria criteria)
        {
            try
            {
                return await _repository.DeleteRoleAsync(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UMS020_UserRoleMapping_Result> AddUsersToRole(UMS020_UserRoleMapping_Criteria data)
        {
            try
            {
                return await _repository.AddUsersToRole(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<UMS020_GetUserRoleMapping_Result> GetUserRoleMapping(UMS020_GetUserRoleMapping_Criteria criteria)
        {
            try
            {
                return await _repository.UMS020_GetUserRoleMapping(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
