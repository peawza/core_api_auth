using Authentication.Repositories;
using Utils.Services;
using static Authentication.Models.CMS040.CMS040;

namespace Authentication.Services
{
    public partial interface ICMS040Service
    {
        //Task<List<CMS040_SearchMiscellaneous_Result>> CMS040_SearchMiscellaneous(CMS040_SearchMiscellaneous_Criteria criteria);
        //Task<CMS040_MiscellaneousDetail_Result?> CMS040_GetMiscellaneousIdAsync(CMS040_MiscellaneousDetail_Criteria criteria);
        //Task<CMS040_DeleteMiscellaneous_Result?> CMS040_DeleteMiscellaneous(CMS040_DeleteMiscellaneous_Criteria criteria);
        //Task<CMS040_InsertMiscellaneous_Result?> CMS040_InsertMiscellaneous(CMS040_InsertMiscellaneous_Criteria criteria);
        //Task<CMS040_UpdateMiscellaneous_Result?> CMS040_UpdateMiscellaneous(CMS040_UpdateMiscellaneous_Criteria criteria);

        //Task<List<CMS040_MiscellaneousType_Result>> GetMiscellaneousTypesAsync(CMS040_MiscellaneousType_Criteria criteria);
    }
    public class CMS040Service : ICMS040Service
    {
        private readonly ICMS040Repository _ICMS040Repository;
        //private readonly BackOfficeAdminDbContext _db;
        private readonly IEmailService _emailService;

        private readonly ICommonRepository _commonRepository;

        //public CMS040Service(ICMS040Repository ICMS040Repository, BackOfficeAdminDbContext db, IEmailService emailService, ICommonRepository common_repository)
        public CMS040Service(ICMS040Repository ICMS040Repository, IEmailService emailService, ICommonRepository common_repository)
        {
            _ICMS040Repository = ICMS040Repository;
            //_db = db;
            _emailService = emailService;
            _commonRepository = common_repository;
        }

        //public async Task<List<CMS040_SearchMiscellaneous_Result>> CMS040_SearchMiscellaneous(CMS040_SearchMiscellaneous_Criteria criteria)
        //{
        //    try
        //    {
        //        var result = await _ICMS040Repository.CMS040_SearchMiscellaneous(criteria);
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public async Task<CMS040_MiscellaneousDetail_Result?> CMS040_GetMiscellaneousIdAsync(CMS040_MiscellaneousDetail_Criteria criteria)
        //{
        //    try
        //    {
        //        return await _ICMS040Repository.CMS040_GetMiscellaneousIdAsync(criteria);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        //public async Task<CMS040_DeleteMiscellaneous_Result?> CMS040_DeleteMiscellaneous(CMS040_DeleteMiscellaneous_Criteria criteria)
        //{
        //    await using var transaction = await _db.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var result = await _ICMS040Repository.CMS040_DeleteMiscellaneous(criteria);

        //        await transaction.CommitAsync();
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}

        //public async Task<CMS040_InsertMiscellaneous_Result?> CMS040_InsertMiscellaneous(CMS040_InsertMiscellaneous_Criteria criteria)
        //{
        //    await using var transaction = await _db.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var result = await _ICMS040Repository.CMS040_InsertMiscellaneous(criteria);

        //        if (result?.StatusCode == "200")
        //            await transaction.CommitAsync();
        //        else
        //            await transaction.RollbackAsync();

        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}

        //public async Task<CMS040_UpdateMiscellaneous_Result?> CMS040_UpdateMiscellaneous(CMS040_UpdateMiscellaneous_Criteria criteria)
        //{
        //    await using var transaction = await _db.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var result = await _ICMS040Repository.CMS040_UpdateMiscellaneous(criteria);
        //        await transaction.CommitAsync();
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}

        //public async Task<List<CMS040_MiscellaneousType_Result>> GetMiscellaneousTypesAsync(CMS040_MiscellaneousType_Criteria criteria)
        //{
        //    try
        //    {
        //        return await _ICMS040Repository.GetMiscellaneousTypesAsync(criteria);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
    }
}
