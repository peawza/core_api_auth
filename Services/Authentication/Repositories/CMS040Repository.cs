using AutoMapper;
using static Authentication.Models.CMS040.CMS040;

namespace Authentication.Repositories
{
    public interface ICMS040Repository
    {
        //Task<List<CMS040_SearchMiscellaneous_Result>> CMS040_SearchMiscellaneous(CMS040_SearchMiscellaneous_Criteria criteria);
        //Task<CMS040_MiscellaneousDetail_Result?> CMS040_GetMiscellaneousIdAsync(CMS040_MiscellaneousDetail_Criteria criteria);
        //Task<CMS040_DeleteMiscellaneous_Result?> CMS040_DeleteMiscellaneous(CMS040_DeleteMiscellaneous_Criteria criteria);
        //Task<CMS040_InsertMiscellaneous_Result?> CMS040_InsertMiscellaneous(CMS040_InsertMiscellaneous_Criteria criteria);
        //Task<CMS040_UpdateMiscellaneous_Result?> CMS040_UpdateMiscellaneous(CMS040_UpdateMiscellaneous_Criteria criteria);

        //Task<List<CMS040_MiscellaneousType_Result>> GetMiscellaneousTypesAsync(CMS040_MiscellaneousType_Criteria criteria);
    }
    public class CMS040Repository : ICMS040Repository
    {
        //private readonly BackOfficeAdminDbContext _db;
        private readonly IMapper _mapper;

        //public CMS040Repository(BackOfficeAdminDbContext db, IMapper mapper)
        public CMS040Repository(IMapper mapper)
        {
            //_db = db;
            _mapper = mapper;
        }
        //public async Task<List<CMS040_MiscellaneousType_Result>> GetMiscellaneousTypesAsync(CMS040_MiscellaneousType_Criteria criteria)
        //{
        //    var query = _db.TmMiscellaneousTypes
        //        .Where(x => !x.DeletedFlag && x.ActiveFlag && x.AddFlag && x.ModifyFlag);

        //    return await query
        //        .Select(x => new CMS040_MiscellaneousType_Result
        //        {
        //            MiscTypeId = x.MiscTypeId,
        //            MiscTypeCode = x.MiscTypeCode,
        //            MiscTypeName = x.MiscTypeName
        //        })
        //        .OrderBy(x => x.MiscTypeCode)
        //        .ToListAsync();
        //}
        //public async Task<List<CMS040_SearchMiscellaneous_Result>> CMS040_SearchMiscellaneous(CMS040_SearchMiscellaneous_Criteria criteria)
        //{
        //    try
        //    {
        //        var query = from mc in _db.TmMiscellaneousValues
        //                    join mt in _db.TmMiscellaneousTypes on mc.MiscTypeId equals mt.MiscTypeId
        //                    //where mt.AddFlag.ToString() == "1"
        //                    //join misc_status in _db.TmMiscellaneousValues on
        //                    //    new { Code = mc.ActiveFlag.ToString(), Type = "MiscCodeStatus" }
        //                    //    equals new { Code = misc_status.MiscCode, Type = misc_status.MiscCode }
        //                    where !mc.DeletedFlag
        //                            && !mt.DeletedFlag
        //                            && (string.IsNullOrEmpty(criteria.MiscValue) || mc.MiscCode == criteria.MiscValue || mc.ValueEn.Contains(criteria.MiscValue))
        //                            && (string.IsNullOrEmpty(criteria.MiscType) || mt.MiscTypeCode == criteria.MiscType)
        //                            && (!criteria.Status.HasValue || mc.ActiveFlag == criteria.Status)
        //                            && mt.AddFlag == true
        //                            && mt.ModifyFlag == true
        //                            && mt.ActiveFlag == true
        //                    orderby mc.MiscTypeId, mc.SeqNo
        //                    select new CMS040_SearchMiscellaneous_Result
        //                    {
        //                        MiscTypeCode = mt.MiscTypeCode,
        //                        MiscTypeName = mt.MiscTypeName,
        //                        SeqNo = mc.SeqNo,
        //                        MiscCode = mc.MiscCode,
        //                        MiscName = mc.ValueEn,
        //                        Value1 = mc.ValueEn,
        //                        Value2 = mc.ValueLocal,
        //                        Description = mc.Description,
        //                        Status = mc.ActiveFlag,
        //                        //TextStatus = misc_status != null ? misc_status.Description : null,
        //                        CreateBy = mc.CreatedBy,
        //                        CreateDate = mc.CreatedDate,
        //                        UpdateBy = mc.UpdatedBy,
        //                        UpdateDate = mc.UpdatedDate
        //                    };

        //        var result = await query.ToListAsync();



        //        return result;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public async Task<CMS040_MiscellaneousDetail_Result?> CMS040_GetMiscellaneousIdAsync(CMS040_MiscellaneousDetail_Criteria criteria)
        //{
        //    var result = await (from value in _db.TmMiscellaneousValues
        //                        join type in _db.TmMiscellaneousTypes
        //                            on value.MiscTypeId equals type.MiscTypeId
        //                        where !value.DeletedFlag && !type.DeletedFlag
        //                              && type.MiscTypeCode == criteria.MiscTypeCode
        //                              && value.MiscCode == criteria.MiscCode
        //                        select new CMS040_MiscellaneousDetail_Result
        //                        {
        //                            MiscCodeId = value.MiscCodeId,
        //                            MiscTypeCode = type.MiscTypeCode,
        //                            MiscCode = value.MiscCode,
        //                            Value1 = value.ValueEn,
        //                            Value2 = value.ValueLocal,
        //                            Description = value.Description,
        //                            SeqNo = value.SeqNo ?? 0,
        //                            ActiveFlag = value.ActiveFlag
        //                        }).FirstOrDefaultAsync();

        //    return result;
        //}

        //public async Task<CMS040_DeleteMiscellaneous_Result?> CMS040_DeleteMiscellaneous(CMS040_DeleteMiscellaneous_Criteria criteria)
        //{
        //    var result = new CMS040_DeleteMiscellaneous_Result();

        //    // ค้นหา record
        //    var record = await (from v in _db.TmMiscellaneousValues
        //                        join t in _db.TmMiscellaneousTypes on v.MiscTypeId equals t.MiscTypeId
        //                        where !v.DeletedFlag
        //                              && !t.DeletedFlag
        //                              && t.MiscTypeCode == criteria.MiscTypeCode
        //                              && v.MiscCode == criteria.MiscCode
        //                        select new { v, t }).FirstOrDefaultAsync();

        //    if (record.t.MiscTypeCode == "NumStation")
        //    {
        //        var DataCheck = await _db.TmConfigurations
        //                    .FirstOrDefaultAsync(u => u.NumStation == Convert.ToInt32(record.v.MiscCode));
        //        if (DataCheck != null)
        //        {
        //            return new CMS040_DeleteMiscellaneous_Result
        //            {
        //                StatusCode = "410",
        //                StatusName = "Gone",
        //                MessageCode = "CannotDelete",
        //                MessageName = "Miscellaneous Cannot deleted"
        //            };
        //        }

        //    }
        //    else if (record.t.MiscTypeCode == "ReminderType")
        //    {
        //        var DataCheck = await _db.TmConfigurations
        //                    .FirstOrDefaultAsync(u => u.ReminderType == record.v.MiscCode);
        //        if (DataCheck != null)
        //        {
        //            return new CMS040_DeleteMiscellaneous_Result
        //            {
        //                StatusCode = "410",
        //                StatusName = "Gone",
        //                MessageCode = "CannotDelete",
        //                MessageName = "Miscellaneous Cannot deleted"
        //            };
        //        }

        //    }

        //    if (record == null)
        //    {
        //        return new CMS040_DeleteMiscellaneous_Result
        //        {
        //            StatusCode = "404",
        //            StatusName = "Not Found",
        //            MessageCode = "E001",
        //            MessageName = "Data not found to delete."
        //        };
        //    }

        //    record.v.DeletedFlag = true;
        //    record.v.DeletedBy = criteria.DeletedBy;
        //    record.v.DeletedDate = DateTime.Now;

        //    await _db.SaveChangesAsync();

        //    return new CMS040_DeleteMiscellaneous_Result
        //    {
        //        StatusCode = "200",
        //        StatusName = "Success",
        //        MessageCode = "S001",
        //        MessageName = "Deleted successfully."
        //    };
        //}

        //public async Task<CMS040_InsertMiscellaneous_Result?> CMS040_InsertMiscellaneous(CMS040_InsertMiscellaneous_Criteria criteria)
        //{
        //    var result = new CMS040_InsertMiscellaneous_Result();


        //    var type = await _db.TmMiscellaneousTypes
        //        .FirstOrDefaultAsync(t => t.MiscTypeCode == criteria.MiscTypeCode && !t.DeletedFlag);

        //    if (type == null)
        //    {
        //        return new CMS040_InsertMiscellaneous_Result
        //        {
        //            StatusCode = "404",
        //            StatusName = "Not Found",
        //            MessageCode = "E001",
        //            MessageName = "Miscellaneous type not found."
        //        };
        //    }

        //    var existing = await _db.TmMiscellaneousValues
        //        .FirstOrDefaultAsync(v =>
        //            v.MiscTypeId == type.MiscTypeId &&
        //            v.MiscCode == criteria.MiscCode);

        //    if (existing != null)
        //    {
        //        if (!existing.DeletedFlag)
        //        {
        //            return new CMS040_InsertMiscellaneous_Result
        //            {
        //                StatusCode = "409",
        //                StatusName = "Duplicate",
        //                MessageCode = "E002",
        //                MessageName = $"Miscellaneous code \"{criteria.MiscCode}\" already exists."
        //            };
        //        }


        //        existing.SeqNo = criteria.SeqNo;
        //        existing.Description = criteria.Description;
        //        existing.ValueEn = criteria.Value1;
        //        existing.ValueLocal = criteria.Value2;
        //        existing.ActiveFlag = criteria.Status;
        //        existing.DeletedFlag = false;
        //        existing.DeletedBy = null;
        //        existing.DeletedDate = null;
        //        existing.UpdatedBy = criteria.CreatedBy;
        //        existing.UpdatedDate = DateTime.Now;

        //        await _db.SaveChangesAsync();

        //        return new CMS040_InsertMiscellaneous_Result
        //        {
        //            StatusCode = "200",
        //            StatusName = "Success",
        //            MessageCode = "S002",
        //            MessageName = "Previously deleted record restored successfully."
        //        };
        //    }


        //    var newValue = new TmMiscellaneousValue
        //    {
        //        MiscTypeId = type.MiscTypeId,
        //        MiscCode = criteria.MiscCode!,
        //        SeqNo = criteria.SeqNo,
        //        Description = criteria.Description,
        //        ValueEn = criteria.Value1,
        //        ValueLocal = criteria.Value2,
        //        ActiveFlag = criteria.Status,
        //        CreatedBy = criteria.CreatedBy,
        //        CreatedDate = DateTime.Now,
        //        DeletedFlag = false
        //    };

        //    _db.TmMiscellaneousValues.Add(newValue);
        //    await _db.SaveChangesAsync();

        //    return new CMS040_InsertMiscellaneous_Result
        //    {
        //        StatusCode = "200",
        //        StatusName = "Success",
        //        MessageCode = "S001",
        //        MessageName = "Inserted successfully."
        //    };
        //}


        //public async Task<CMS040_UpdateMiscellaneous_Result?> CMS040_UpdateMiscellaneous(CMS040_UpdateMiscellaneous_Criteria criteria)
        //{
        //    var result = new CMS040_UpdateMiscellaneous_Result();

        //    var type = await _db.TmMiscellaneousTypes
        //        .FirstOrDefaultAsync(t => t.MiscTypeCode == criteria.MiscTypeCode && !t.DeletedFlag);

        //    if (type == null)
        //    {
        //        return new CMS040_UpdateMiscellaneous_Result
        //        {
        //            StatusCode = "404",
        //            StatusName = "Not Found",
        //            MessageCode = "E001",
        //            MessageName = "Miscellaneous type not found."
        //        };
        //    }

        //    var value = await _db.TmMiscellaneousValues
        //        .FirstOrDefaultAsync(v => v.MiscTypeId == type.MiscTypeId && v.MiscCode == criteria.MiscCode && !v.DeletedFlag);

        //    if (value == null)
        //    {
        //        return new CMS040_UpdateMiscellaneous_Result
        //        {
        //            StatusCode = "404",
        //            StatusName = "Not Found",
        //            MessageCode = "E002",
        //            MessageName = "Miscellaneous value not found."
        //        };
        //    }

        //    value.SeqNo = criteria.SeqNo;
        //    value.Description = criteria.Description;
        //    value.ValueEn = criteria.Value1;
        //    value.ValueLocal = criteria.Value2;
        //    value.ActiveFlag = criteria.Status;
        //    value.UpdatedBy = criteria.UpdatedBy;
        //    value.UpdatedDate = DateTime.Now;

        //    await _db.SaveChangesAsync();

        //    return new CMS040_UpdateMiscellaneous_Result
        //    {
        //        StatusCode = "200",
        //        StatusName = "Success",
        //        MessageCode = "UpdateSuccess",
        //        MessageName = "Updated successfully."
        //    };
        //}
    }
}
