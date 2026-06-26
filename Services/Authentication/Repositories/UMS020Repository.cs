using Application;
using Application.Models;
using ApplicationDB.Models.Application;
using Authentication.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static Authentication.Models.UMS020.UMS020;

namespace Authentication.Repositories
{
    public interface IUMS020Repository
    {
        Task<List<MS020_SearchRole_Result>> UMS020_SearchRole(UMS020_SearchRole_Criteria criteria);
        Task<UMS020_AddRole_Result> UMS020_AddRole(UMS020_AddRole_Criteria criteria);
        Task<UMS020_UpdateRole_Result> UMS020_UpdateRole(UMS020_UpdateRole_Criteria criteria);
        Task<UMS020_DeleteRole_Result> DeleteRoleAsync(UMS020_DeleteRole_Criteria criteria);
        Task<UMS020_UserRoleMapping_Result> AddUsersToRole(UMS020_UserRoleMapping_Criteria data);
        Task<UMS020_GetUserRoleMapping_Result> UMS020_GetUserRoleMapping(UMS020_GetUserRoleMapping_Criteria criteria);
    }
    public class UMS020Repository : IUMS020Repository
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _db;
        //private readonly SystemDbContext _db;
        private readonly IMapper _mapper;
        public UMS020Repository(ApplicationDbContext db, IMapper mapper, ApplicationUserManager userManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            this._db = _db;
        }

        public async Task<List<MS020_SearchRole_Result>> UMS020_SearchRole(UMS020_SearchRole_Criteria criteria)
        {
            var query = _db.ApplicationRoles.AsQueryable();
            if (!string.IsNullOrWhiteSpace(criteria.GroupName))
            {
                query = query.Where(r => r.Name.Contains(criteria.GroupName));
            }
            if (!string.IsNullOrWhiteSpace(criteria.GroupDescription))
            {
                query = query.Where(r => r.Description.Contains(criteria.GroupDescription));
            }
            query = query.Where(r => r.Name != "Super Admin");

            query = query.Where(r => (!criteria.Status.HasValue || r.IsActive == criteria.Status));
            var result = await query
                .Select(r => new MS020_SearchRole_Result
                {
                    Id = r.Id,
                    Name = r.Name,
                    NormalizedName = r.NormalizedName,
                    Description = r.Description,
                    IsActive = r.IsActive,
                    CreateBy = r.CreateBy,
                    CreateDate = r.CreateDate,
                    UpdateBy = r.UpdateBy,
                    UpdateDate = r.UpdateDate,
                    Detail = (List<CMS020_SearchDetail_Result>)(from us in _db.Users
                                                                join usi in _db.UserInfos on us.Id equals usi.Id
                                                                join d in _db.Departments on usi.DepartmentCode equals d.DepartmentCode into dGroup
                                                                from d in dGroup.DefaultIfEmpty()
                                                                join ur in _db.UserRoles on us.Id equals ur.UserId into urGroup
                                                                from ur in urGroup.DefaultIfEmpty()
                                                                where us.ActiveFlag == true && (ur.RoleId == r.Id)
                                                                //((ur == null && ur.RoleId == r.Id) || (us.UserName != criteria.UserName && ur.RoleId == r.Id))
                                                                select new CMS020_SearchDetail_Result
                                                                {
                                                                    UserId = us.Id,
                                                                    Username = usi.UserName,
                                                                    name = usi.FirstName + " " + usi.LastName,
                                                                    DepartmentCode = usi.DepartmentCode,
                                                                    DisplayDepartmentCode = d != null ? d.DepartmentName : null,
                                                                    PositionCode = usi.PositionCode,
                                                                    DisplayPosition = _db.Positions
                                                                                      .Where(p => p.PositionCode == usi.PositionCode)
                                                                                      .Select(p => p.PositionName)
                                                                                      .FirstOrDefault()
                                                                })

                })
                .OrderBy(r => r.Name)
                .ToListAsync();
            if (!string.IsNullOrWhiteSpace(criteria.UserName))
            {
                result = result
                .Where(r => r.Detail.Any(d => d.Username.Contains(criteria.UserName)))
                .ToList();

            }





            return result;
        }




        public async Task<UMS020_AddRole_Result> UMS020_AddRole(UMS020_AddRole_Criteria criteria)
        {
            var result = new UMS020_AddRole_Result();

            try
            {
                // ตรวจสอบชื่อซ้ำโดยใช้ NormalizedName (อิงตามมาตรฐาน Identity)
                var normalizedName = criteria.Name.Trim().ToUpperInvariant();
                bool roleExists = await _db.ApplicationRoles.AnyAsync(r => r.NormalizedName == normalizedName);
                if (roleExists)
                {
                    return new UMS020_AddRole_Result
                    {
                        StatusCode = "409",
                        StatusName = "Conflict",
                        MessageCode = "ROLE002",
                        MessageName = $"Role name '{criteria.Name}' already exists."
                    };
                }

                var newRole = new ApplicationRole
                {
                    Name = criteria.Name.Trim(),
                    NormalizedName = normalizedName,
                    Description = criteria.Description,
                    IsActive = criteria.IsActive == true ? true : false,
                    CreateBy = criteria.CreateBy,
                    CreateDate = DateTime.Now,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                _db.ApplicationRoles.Add(newRole);
                await _db.SaveChangesAsync();

                return new UMS020_AddRole_Result
                {
                    StatusCode = "200",
                    StatusName = "Success",
                    MessageCode = "ROLE000",
                    MessageName = "add role successfully."
                };
            }
            catch (Exception ex)
            {
                return new UMS020_AddRole_Result
                {
                    StatusCode = "500",
                    StatusName = "Internal Server Error",
                    MessageCode = "ROLE999",
                    MessageName = "An unexpected error occurred: " + ex.Message
                };
            }
        }


        public async Task<UMS020_UpdateRole_Result> UMS020_UpdateRole(UMS020_UpdateRole_Criteria criteria)
        {
            var result = new UMS020_UpdateRole_Result();

            try
            {
                var role = await _db.ApplicationRoles.FirstOrDefaultAsync(r => r.Id == criteria.Id);
                if (role == null)
                {
                    return new UMS020_UpdateRole_Result
                    {
                        StatusCode = "404",
                        StatusName = "Not Found",
                        MessageCode = "ROLE004",
                        MessageName = $"Role with ID {criteria.Id} was not found."
                    };
                }

                // อัปเดตข้อมูล
                role.Name = criteria.Name.Trim();
                role.NormalizedName = criteria.Name.Trim().ToUpperInvariant();
                role.Description = criteria.Description;
                role.IsActive = criteria.IsActive == true ? true : false;
                role.UpdateBy = criteria.UpdateBy;
                role.UpdateDate = criteria.UpdateDate;

                await _db.SaveChangesAsync();

                return new UMS020_UpdateRole_Result
                {
                    StatusCode = "200",
                    StatusName = "Success",
                    MessageCode = "ROLE005",
                    MessageName = ""
                };
            }
            catch (Exception ex)
            {

                return new UMS020_UpdateRole_Result
                {
                    StatusCode = "500",
                    StatusName = "Internal Server Error",
                    MessageCode = "ROLE999",
                    MessageName = "An unexpected error occurred: " + ex.Message
                };
            }
        }


        public async Task<UMS020_DeleteRole_Result> DeleteRoleAsync(UMS020_DeleteRole_Criteria criteria)
        {
            var result = new UMS020_DeleteRole_Result();

            try
            {
                var role = await _db.ApplicationRoles.FirstOrDefaultAsync(r => r.Id == criteria.Id);
                if (role == null)
                {
                    result.StatusCode = "404";
                    result.StatusName = "Not Found";
                    result.MessageCode = "ROLE011";
                    result.MessageName = $"Role ID {criteria.Id} not found.";
                    return result;
                }

                // ตรวจสอบว่ามี user ถูกแมปกับ role นี้อยู่ไหม
                var usersInRole = await _db.UserRoles
                    .AnyAsync(ur => ur.RoleId == criteria.Id);

                if (usersInRole)
                {
                    result.StatusCode = "409";
                    result.StatusName = "Conflict";
                    result.MessageCode = "ROLE012";
                    result.MessageName = "Conflict";
                    return result;
                }

                _db.ApplicationRoles.Remove(role);
                await _db.SaveChangesAsync();

                result.StatusCode = "200";
                result.StatusName = "Success";
                result.MessageCode = "ROLE000";
                result.MessageName = "DeleteSuccess";
                return result;
            }
            catch (Exception ex)
            {
                result.StatusCode = "500";
                result.StatusName = "Error";
                result.MessageCode = "ROLE999";
                result.MessageName = "Unexpected error occurred: " + ex.Message;
                return result;
            }
        }


        public async Task<UMS020_UserRoleMapping_Result> AddUsersToRole(UMS020_UserRoleMapping_Criteria data)
        {
            try
            {
                if (data == null)
                {
                    return new UMS020_UserRoleMapping_Result
                    {
                        StatusCode = "400",
                        StatusName = "Bad Request",
                        MessageCode = "USR001",
                        MessageName = "Invalid input data."
                    };
                }

                var role = await _db.ApplicationRoles.FirstOrDefaultAsync(r => r.Id == data.RoleId);
                if (role == null)
                {
                    return new UMS020_UserRoleMapping_Result
                    {
                        StatusCode = "404",
                        StatusName = "Not Found",
                        MessageCode = "USR002",
                        MessageName = $"Role ID {data.RoleId} not found."
                    };
                }

                role.UpdateBy = data.User;
                role.UpdateDate = DateTime.Now;
                var userRoles = await _db.UserRoles.ToListAsync();
                var toRemove = userRoles.Where(ur => ur.RoleId == data.RoleId).ToList();
                _db.UserRoles.RemoveRange(toRemove);
                await _db.SaveChangesAsync();
                if (data.Users == null || !data.Users.Any())
                {
                    //var toRemove = userRoles.Where(ur => ur.RoleId == data.RoleId).ToList();
                    //_db.UserRoles.RemoveRange(toRemove);
                }
                else
                {
                    var userIdSet = new HashSet<string>(data.Users);

                    //var removeList = userRoles
                    //    .Where(ur => ur.RoleId == data.RoleId && !userIdSet.Contains(ur.UserId))
                    //    .ToList();
                    //_db.UserRoles.RemoveRange(removeList);
                    //await _db.SaveChangesAsync();
                    foreach (var userId in userIdSet)
                    {
                        if (userId == null)
                            continue;


                        var exists = await _db.UserRoles
                                .AnyAsync(x => x.UserId == userId && x.RoleId == data.RoleId);

                        if (!exists)
                        {
                            _db.UserRoles.Add(new tb_UserRole
                            {
                                UserId = userId,
                                RoleId = data.RoleId
                            });
                        }

                        /*
                        var existingRoles = userRoles.Where(ur => ur.UserId == userId).ToList();

                        if (!existingRoles.Any())
                        {
                            _db.UserRoles.Add(new tb_UserRole
                            {
                                UserId = userId,
                                RoleId = data.RoleId
                            });
                        }
                        else
                        {
                            foreach (var ur in existingRoles.Where(x => x.RoleId != data.RoleId))
                            {
                                _db.UserRoles.Remove(ur);
                                _db.UserRoles.Add(new tb_UserRole
                                {
                                    UserId = userId,
                                    RoleId = data.RoleId
                                });
                            }
                        }
                        */
                    }
                }

                await _db.SaveChangesAsync();

                return new UMS020_UserRoleMapping_Result
                {
                    StatusCode = "200",
                    StatusName = "Success",
                    MessageCode = "USR000",
                    MessageName = "Users successfully mapped to role."
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<UMS020_GetUserRoleMapping_Result> UMS020_GetUserRoleMapping(UMS020_GetUserRoleMapping_Criteria criteria)
        {
            UMS020_GetUserRoleMapping_Result result = new UMS020_GetUserRoleMapping_Result();
            var queryoutrole = from us in _db.Users
                               join usi in _db.UserInfos on us.Id equals usi.Id
                               join d in _db.Departments on usi.DepartmentCode equals d.DepartmentCode into dGroup
                               from d in dGroup.DefaultIfEmpty()
                               join ur in _db.UserRoles
                                on new { UserId = us.Id, RoleId = criteria.RoleId }
                                equals new { ur.UserId, ur.RoleId } into urGroup
                               from ur in urGroup.DefaultIfEmpty()
                               where us.ActiveFlag == true && ur == null
                               select new UMS020_UserList_Result
                               {
                                   UserId = us.Id,
                                   UserName = usi.UserName,
                                   DisplayFullName = usi.FirstName + ' ' + usi.LastName,
                                   DisplayDepartment = d != null ? d.DepartmentName : null
                               };
            result.UserListOutRole = await queryoutrole.OrderBy(o => o.UserName).ToListAsync();
            var queryinrole = from us in _db.Users
                              join usi in _db.UserInfos on us.Id equals usi.Id
                              join d in _db.Departments on usi.DepartmentCode equals d.DepartmentCode into dGroup
                              from d in dGroup.DefaultIfEmpty()
                              join ur in _db.UserRoles on us.Id equals ur.UserId into urGroup
                              from ur in urGroup.DefaultIfEmpty()
                              where us.ActiveFlag == true && ur.RoleId == criteria.RoleId
                              select new UMS020_UserList_Result
                              {
                                  UserId = us.Id,
                                  UserName = usi.UserName,
                                  DisplayFullName = usi.FirstName + ' ' + usi.LastName,
                                  DisplayDepartment = d != null ? d.DepartmentName : null
                              };
            result.UserListInRole = await queryinrole.ToListAsync();
            return result;
        }
    }
}
