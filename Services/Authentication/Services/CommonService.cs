using Authentication.Repositories;
using static Authentication.Models.CommonModels;

namespace Authentication.Services
{
    public partial interface ICommonService
    {
        Task<List<Common_Department_Result>> GetDepartmentsAsync(Common_Department_Criteria criteria);
        Task<List<Common_Position_Result>> GetPositionsAsync(Common_Position_Criteria criteria);
        Task<List<Common_Role_Result>> GetRolesAsync(Common_Role_Criteria criteria);
        Task<List<Common_MiscellaneousModules_Result>> GetMiscellaneousModules(Common_MiscellaneousModules_Criteria criteria);
        //GetMiscellaneousModules
    }

    public class CommonService : ICommonService
    {
        private readonly ICommonRepository _commonRepository;

        public CommonService(ICommonRepository common_repository)
        {
            _commonRepository = common_repository;
        }

        public Task<List<Common_MiscellaneousModules_Result>> GetMiscellaneousModules(Common_MiscellaneousModules_Criteria criteria)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Common_Department_Result>> GetDepartmentsAsync(Common_Department_Criteria criteria)
        {
            try
            {
                return await _commonRepository.GetDepartmentsAsync(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<Common_Position_Result>> GetPositionsAsync(Common_Position_Criteria criteria)
        {
            try
            {
                return await _commonRepository.GetPositionsAsync(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<Common_Role_Result>> GetRolesAsync(Common_Role_Criteria criteria)
        {
            try
            {
                return await _commonRepository.GetRolesAsync(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }



    }

}
