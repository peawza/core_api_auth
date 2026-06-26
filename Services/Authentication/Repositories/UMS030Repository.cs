using Application;
using Application.Models;
using ApplicationDB.Models.System;
using Authentication.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static Authentication.Models.UMS030.UMS030;

namespace Authentication.Repositories
{
    public interface IUMS030Repository
    {
        Task<IEnumerable<GroupPermissionDataView>> ListByUserGroup(string userGroupId);
        Task<IEnumerable<ts_ScreenFunction>> ListScreenFunctions();
        Task<UMS030_UpdatePermission_Result> UpdatePermission(UMS030_UpdatePermission_Criteria criteria);
        Task<List<UMS030_GetRoles_Result>>? GetRoles(UMS030_GetRoles_Criteria criteria);
        Task<List<UMS030_GetModule_Result>>? GetModule(UMS030_GetModule_Criteria criteria);
    }
    public class UMS030Repository : IUMS030Repository
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _db;
        private readonly SystemDbContext _systemDb;
        private readonly IMapper _mapper;
        public UMS030Repository(ApplicationDbContext db, IMapper mapper, ApplicationUserManager userManager, SystemDbContext systemDb)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _systemDb = systemDb;
        }

        public async Task<IEnumerable<GroupPermissionDataView>> ListByUserGroup(string? userGroupId)
        {
            //var query = from m in _systemDb.Module
            //            join s in _systemDb.Screen on m.ModuleCode equals s.ModuleCode
            //            where s.PermissionFlag == true
            //            join p in _db.GroupPermissions
            //                //.Where(x => x.GroupId == userGroupId)
            //                on s.ScreenId equals p.ScreenId into gp
            //            from p in gp.DefaultIfEmpty() // LEFT JOIN
            //            orderby m.Seq, s.ScreenId
            //            select new GroupPermissionDataView
            //            {
            //                ModuleCode = m.ModuleCode,
            //                ModuleName = m.ModuleNameEN, // or .ModuleName_EN depending on language
            //                Seq = m.Seq,
            //                ScreenId = s.ScreenId,
            //                ScreenName = s.NameEN,
            //                ScreenFunctionCode = s.FunctionCode,
            //                PermissionFunctionCode = p != null ? p.FunctionCode : 0
            //            };

            //var result = await query.ToListAsync();
            //return result.AsEnumerable();
            // Query จาก _systemDb
            var screens = await (
                from m in _systemDb.Module
                join s in _systemDb.Screen on m.ModuleCode equals s.ModuleCode
                join sm in _systemDb.SubModule on s.SubModuleCode equals sm.SubModuleCode into smGroup
                from sub in smGroup.DefaultIfEmpty()
                where s.PermissionFlag == true
                orderby m.Seq, sub.Seq, s.Seq
                select new
                {
                    ModuleCode = m.ModuleCode,
                    ModuleNameEN = m.ModuleNameEN,
                    SubModuleCode = sub != null ? sub.SubModuleCode : "##",
                    SubModuleEN = sub != null ? sub.SubModuleNameEN : "##",
                    Seq = m.Seq,
                    ScreenId = s.ScreenId,
                    ScreenNameEN = s.NameEN,
                    FunctionCode = s.FunctionCode
                }
            ).ToListAsync();

            // Query จาก _db
            var groupPermissions = await _db.GroupPermissions
                .Where(x => x.GroupId == userGroupId)
                .ToListAsync();

            /*
             public string SubModuleCode { get; set; }
            public string SubModuleName { get; set; }
             */
            // Join ด้วย LINQ to Objects
            var result = from s in screens
                         join p in groupPermissions on s.ScreenId equals p.ScreenId into gp
                         from p in gp.DefaultIfEmpty()
                         orderby s.ModuleCode, s.SubModuleCode
                         select new GroupPermissionDataView
                         {
                             ModuleCode = s.ModuleCode,
                             ModuleName = s.ModuleNameEN,
                             SubModuleCode = s.SubModuleCode,
                             SubModuleName = s.SubModuleEN,
                             Seq = s.Seq,
                             ScreenId = s.ScreenId,
                             ScreenName = s.ScreenNameEN,
                             ScreenFunctionCode = s.FunctionCode,
                             PermissionFunctionCode = p != null ? p.FunctionCode : 0
                         };

            return result;
        }

        public async Task<IEnumerable<ts_ScreenFunction>> ListScreenFunctions()
        {
            return await _systemDb.ScreenFunctions.ToListAsync();
        }

        public async Task<UMS030_UpdatePermission_Result> UpdatePermission(UMS030_UpdatePermission_Criteria criteria)
        {
            try
            {

                var permissions = criteria.GroupPermissionData.Select(t => new UMS030_UpdatePermission_list_Criteria
                {
                    GroupId = t.GroupId,
                    ScreenId = t.ScreenId,
                    FunctionCode = t.FunctionCode,
                    CreateDate = t.CreateDate,
                    CreateBy = t.CreateBy,
                    UpdateDate = t.UpdateDate,
                    UpdateBy = t.UpdateBy
                }).ToList();


                if (permissions.Count != 0)
                {
                    var userGroupIds = permissions.Select(t => t.GroupId).Distinct().ToList();
                    foreach (var userGroupId in userGroupIds)
                    {
                        var permissionToEdits = await _db.GroupPermissions
                            .Where(t => t.GroupId == userGroupId)
                            .ToListAsync();

                        var existingScreens = permissionToEdits.Select(t => t.ScreenId).ToHashSet();
                        var newPermissions = permissions.Where(t => t.GroupId == userGroupId).ToList();
                        var screenIdsToKeep = new HashSet<string>();

                        foreach (var source in newPermissions)
                        {
                            var existing = permissionToEdits.SingleOrDefault(t => t.ScreenId == source.ScreenId);
                            if (existing == null)
                            {
                                var newPermission = new tb_GroupPermission
                                {
                                    GroupId = source.GroupId,
                                    ScreenId = source.ScreenId,
                                    FunctionCode = source.FunctionCode,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                    CreateBy = source.CreateBy, // สมมุติว่ามี property นี้ใน criteria
                                    UpdateBy = source.UpdateBy  // ถ้ามี
                                };

                                _db.GroupPermissions.Add(newPermission);
                            }
                            else
                            {
                                screenIdsToKeep.Add(source.ScreenId);
                                if (existing.FunctionCode != source.FunctionCode)
                                {
                                    existing.FunctionCode = source.FunctionCode;
                                    existing.UpdateDate = DateTime.Now;
                                    _db.GroupPermissions.Update(existing);
                                }
                            }
                        }

                        var toDelete = permissionToEdits
                            .Where(p => !screenIdsToKeep.Contains(p.ScreenId))
                            .ToList();

                        _db.GroupPermissions.RemoveRange(toDelete);
                    }

                }
                else
                {
                    var permissionToEdits = await _db.GroupPermissions
                            .Where(t => t.GroupId == criteria.GroupId)
                            .ToListAsync();

                    var toDelete = permissionToEdits
                           //.Where(p => !screenIdsToKeep.Contains(p.ScreenId))
                           .ToList();

                    _db.GroupPermissions.RemoveRange(toDelete);

                }


                await _db.SaveChangesAsync();

                return new UMS030_UpdatePermission_Result
                {
                    StatusCode = "SUCCESS",
                    StatusName = "สำเร็จ",
                    MessageCode = "UMS030_UpdatePermission_SUCCESS",
                    MessageName = "สร้างผู้ใช้เรียบร้อยแล้ว"
                };
            }
            catch (Exception ex)
            {

                return new UMS030_UpdatePermission_Result
                {
                    StatusCode = "ERROR",
                    StatusName = "ผิดพลาด",
                    MessageCode = "UMS030_UpdatePermission_EXCEPTION",
                    MessageName = ex.Message
                };
            }
        }
        public async Task<List<UMS030_GetRoles_Result>>? GetRoles(UMS030_GetRoles_Criteria criteria)
        {
            var query = from r in _db.ApplicationRoles
                        select new UMS030_GetRoles_Result
                        {
                            Id = r.Id,
                            Name = r.Name,
                            NormalizedName = r.NormalizedName,
                            Description = r.Description
                        };
            return await query.ToListAsync();
        }
        public async Task<List<UMS030_GetModule_Result>>? GetModule(UMS030_GetModule_Criteria criteria)
        {
            var query = from m in _systemDb.Module
                        select new UMS030_GetModule_Result
                        {
                            Id = m.Id,
                            ModuleCode = m.ModuleCode,
                            ModuleNameEN = m.ModuleNameEN,
                            ModuleNameTH = m.ModuleNameTH,
                            Seq = m.Seq,
                            Icon = m.IconClass
                        };
            return await query.ToListAsync();
        }
    }
}
