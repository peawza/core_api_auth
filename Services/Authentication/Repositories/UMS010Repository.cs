using Application;
using Application.Models;
using ApplicationDB.Models.Application;
using Authentication.Services;
using AutoMapper;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Utils.Helper;
using Utils.Models;
using Utils.Services;
using static Authentication.Models.NewFolder.UMS010;

namespace Authentication.Repositories
{
    public interface IUMS010Repository
    {
        Task<List<UMS010_Search_Result>> UMS010_Search(UMS010_Search_Criteria criteria);

        Task<GetUserID_Info_Result?> GetUserID_Info(GetUserID_Info_Criteria criteria);

        Task<UMS010_CreateUser_Result> CreateUser(UMS010_CreateUser_Criteria criteria);

        Task<UMS010_UpdateUser_Result> UpdateUser(UMS010_UpdateUser_Criteria criteria);

        Task<UMS010_DeleteUser_Result> DeleteUser(UMS010_DeleteUser_Criteria criteria);

        Task<UMS010_UpdateUser_Result> AdminChangePassword(UMS010_AdminChangePassword_Criteria criteria);
        Task<UMS010_UpdateUser_Result> ChangePassword(UMS010_ChangePassword_Criteria criteria);

        Task<UMS010_ForgotPassword_Result> ForgotPassword(UMS010_ForgotPassword_Criteria criteria);

        Task<UMS010_FistLogin_Result> FistLogin(UMS010_FistLogin_Criteria criteria);
        Task<SendEmailFirstLogin_Result> SendEmailFirstLogin(SendEmailFirstLogin_Criteria criteria);
    }
    public class UMS010Repository : IUMS010Repository
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _db;
        private readonly SystemDbContext _systemDbContext;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public UMS010Repository(ApplicationDbContext db, SystemDbContext systemDbContext, IMapper mapper
            , ApplicationUserManager userManager, IEmailService emailService
            , IConfiguration configuration
            )
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _systemDbContext = systemDbContext;
            _emailService = emailService;
            _configuration = configuration;
        }
        private async Task<int> GetUserNumberByUserNameAsync(string userName)
        {
            var userInfo = await _db.UserInfos
                .Where(x => x.UserName == userName)
                .Select(x => (int)x.UserNumber)
                .FirstOrDefaultAsync();

            return userInfo;
        }

        public async Task<List<UMS010_Search_Result>> UMS010_Search(UMS010_Search_Criteria criteria)
        {
            var roleId = await _db.Roles.Where(w => w.Name == criteria.PermissionGroup).Select(s => s.Id).FirstOrDefaultAsync();

            var query = from user in _db.ApplicationUsers
                        join info in _db.UserInfos on user.Id equals info.Id
                        //join dep in _db.Departments on info.DepartmentCode equals dep.DepartmentCode into departmentgroup
                        //from dep in departmentgroup.DefaultIfEmpty()
                        join posi in _db.Positions on info.PositionCode equals posi.PositionCode into positiongroup
                        from posi in positiongroup.DefaultIfEmpty()
                            // Optional: Join เพื่อแสดงชื่อของผู้ที่สร้าง/อัปเดต
                            //join createdBy in _db.UserInfos on info.CreatedBy.ToString() equals createdBy.Id into createdByGroup
                            //from createdBy in createdByGroup.DefaultIfEmpty()

                            //join updatedBy in _db.UserInfos on info.UpdatedBy.ToString() equals updatedBy.Id into updatedByGroup
                            //from updatedBy in updatedByGroup.DefaultIfEmpty()

                        where (string.IsNullOrEmpty(criteria.UserName) || info.UserName.Contains(criteria.UserName))
                              && (string.IsNullOrEmpty(criteria.Name) || (info.FirstName.Contains(criteria.Name) || (info.LastName.Contains(criteria.Name))))
                              && (string.IsNullOrEmpty(criteria.Department) || info.DepartmentCode.Equals(criteria.Department))
                              && (!criteria.ActiveFlag.HasValue || user.ActiveFlag == criteria.ActiveFlag)
                              && (string.IsNullOrWhiteSpace(roleId)
                                || _db.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == roleId))
                              && (posi.PositionCode != "00")


                        select new UMS010_Search_Result
                        {
                            Id = user.Id,
                            UserNumber = info.UserNumber,
                            UserName = info.UserName,
                            FirstName = info.FirstName,
                            LastName = info.LastName,
                            DisplayName = info.FirstName + " " + info.LastName,
                            DepartmentCode = info.DepartmentCode,
                            DisplayDepartment = info.DepartmentCode,
                            PositionCode = posi != null ? posi.PositionCode : null,
                            DisplayPosition = posi != null ? posi.PositionName : null,
                            PermissionGroupId = null,
                            DisplayPermissionGroup = null,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            IsActive = user.ActiveFlag,
                            Remark = info.Remark,
                            LanguageCode = info.LanguageCode,
                            CreateBy = info.CreatedBy,
                            CreateDate = info.CreateDate,
                            UpdateDate = info.UpdateDate,
                            UpdateBy = info.UpdatedBy,
                            LastLoginDate = user.LastLoginDate,
                            LastChangedPassword = user.LastUpdatePasswordDate
                            //CreateBy = createdBy != null ? createdBy.UserName : info.CreatedBy.ToString(),
                            //UpdateBy = updatedBy != null ? updatedBy.UserName : info.UpdatedBy.ToString()
                        };
            var users = await query.ToListAsync();
            var userIds = users.Select(t => t.Id).ToList();
            var userRoles = (from ur in _db.UserRoles
                             join r in _db.Roles on ur.RoleId equals r.Id
                             where userIds.Contains(ur.UserId)
                             select new
                             {
                                 ur.UserId,
                                 ur.RoleId,
                                 RoleName = r.Name // หรือฟิลด์ชื่อ role
                             })
                .ToArray();
            foreach (var user in users)
            {
                var roles = userRoles.Where(t => t.UserId == user.Id);
                user.DisplayPermissionGroup = string.Join(",", roles.Select(t => t.RoleName).ToArray());
            }
            return users;
        }


        public async Task<GetUserID_Info_Result?> GetUserID_Info(GetUserID_Info_Criteria criteria)
        {
            string userId = criteria.UserId;

            var query = from user in _db.ApplicationUsers
                        join info in _db.UserInfos on user.Id equals info.Id
                        //join dep in _db.Departments on info.DepartmentCode equals dep.DepartmentCode into departmentgroup
                        //from dep in departmentgroup.DefaultIfEmpty()
                        join posi in _db.Positions on info.PositionCode equals posi.PositionCode into positiongroup
                        from posi in positiongroup.DefaultIfEmpty()

                        where user.Id == userId

                        select new GetUserID_Info_Result
                        {
                            Id = user.Id,
                            UserNumber = info.UserNumber,
                            UserName = info.UserName,
                            FirstName = info.FirstName,
                            LastName = info.LastName,
                            DepartmentCode = info.DepartmentCode,
                            PositionCode = posi != null ? posi.PositionCode : null,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Remark = info.Remark,
                            LanguageCode = info.LanguageCode,
                            CreateDate = info.CreateDate,
                            CreateBy = info.CreatedBy,
                            UpdateDate = info.UpdateDate,
                            UpdateBy = info.UpdatedBy,
                            FirstLoginFlag = user.FirstLoginFlag,
                            //FirstLoginFlag = false,
                            ActiveFlag = user.ActiveFlag,
                            SystemAdminFlag = user.SystemAdminFlag,
                            PasswordAge = user.PasswordAge,
                            LastLoginDate = user.LastLoginDate,
                            LastUpdatePasswordDate = user.LastUpdatePasswordDate,
                            ActiveDate = user.ActiveDate,
                            InActiveDate = user.InActiveDate
                        };

            return await query.FirstOrDefaultAsync();
        }


        public async Task<UMS010_CreateUser_Result> CreateUser(UMS010_CreateUser_Criteria criteria)
        {


            try
            {
                var superAdminCode = "00";
                var superAdminName = "Super Admin";


                var existsPositions = await _db.Positions
                    .AnyAsync(p => p.PositionCode == superAdminCode);

                if (!existsPositions)
                {
                    _db.Positions.Add(new tb_Position
                    {
                        PositionCode = superAdminCode,
                        PositionName = superAdminName,
                    });

                    await _db.SaveChangesAsync();
                }

                superAdminCode = "00";
                superAdminName = "Admin System ";
                var existsDepartments = await _db.Departments
                     .AnyAsync(p => p.DepartmentCode == superAdminCode);

                if (!existsDepartments)
                {
                    _db.Departments.Add(new tb_Department
                    {
                        DepartmentCode = superAdminCode,
                        DepartmentName = superAdminName,
                    });

                    await _db.SaveChangesAsync();
                }

                var GroupAdminName = "Super Admin";
                var GroupAdminDescription = "Full system access";
                var existsRoles = await _db.ApplicationRoles
                     .AnyAsync(p => p.Name == GroupAdminName);




                if (!existsRoles)
                {
                    var role = new ApplicationRole
                    {
                        Name = GroupAdminName,
                        NormalizedName = GroupAdminName.ToUpper(),
                        IsActive = true,
                        Description = GroupAdminDescription
                    };

                    _db.ApplicationRoles.Add(role);
                    await _db.SaveChangesAsync();


                    var insertedGroupId = role.Id;

                    var screenIds = new[] { "UMS010", "UMS020", "UMS030", "CMS040" };
                    foreach (var screenId in screenIds)
                    {
                        var newPermission = new tb_GroupPermission
                        {
                            GroupId = insertedGroupId,
                            ScreenId = screenId,
                            FunctionCode = 63,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            CreateBy = "SYSTEM"
                        };
                        _db.GroupPermissions.Add(newPermission);
                    }
                    await _db.SaveChangesAsync();

                }



                bool firstLogin = _configuration.GetValue<bool>("Authentication:FirstLogin");
                bool otpLogin = _configuration.GetValue<bool>("Authentication:OTPLogin");
                bool sendSmsOtp = _configuration.GetValue<bool>("Authentication:SendSMSOTP");
                bool sendEmailOtp = _configuration.GetValue<bool>("Authentication:SendEmailOTP");
                if (firstLogin == true)
                {
                    criteria.Password = PasswordHelper.GeneratePassword(8);

                }
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = criteria.UserName,
                    Email = criteria.Email,
                    PhoneNumber = criteria.PhoneNumer,
                    FirstLoginFlag = firstLogin,
                    ActiveFlag = criteria.ActiveFlag,
                    SystemAdminFlag = criteria.SystemAdminFlag,
                    OTPLogin = otpLogin,
                    SendSMSOTP = sendSmsOtp,
                    SendEmailOTP = sendEmailOtp,

                };




                var result = await _userManager.CreateAsync(appUser, criteria.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.ToList();

                    // เช็คชื่อผู้ใช้งานซ้ำ
                    var duplicateUserNameError = errors
                        .FirstOrDefault(e =>
                            string.Equals(e.Code, "DuplicateUserName", StringComparison.OrdinalIgnoreCase));

                    // เช็คอีเมลซ้ำ
                    var duplicateEmailError = errors
                        .FirstOrDefault(e =>
                            string.Equals(e.Code, "DuplicateEmail", StringComparison.OrdinalIgnoreCase));

                    // ถ้าซ้ำทั้งคู่
                    if (duplicateUserNameError != null && duplicateEmailError != null)
                    {
                        return new UMS010_CreateUser_Result
                        {
                            StatusCode = "400",
                            StatusName = "ไม่สำเร็จ",
                            MessageCode = "UMS010_USERNAME_EMAIL_DUPLICATE",
                            MessageName = "ชื่อผู้ใช้งานและอีเมลนี้ถูกใช้ลงทะเบียนแล้ว กรุณาเปลี่ยนทั้งชื่อผู้ใช้งานและอีเมล"
                        };
                    }

                    // ชื่อผู้ใช้งานซ้ำอย่างเดียว
                    if (duplicateUserNameError != null)
                    {
                        return new UMS010_CreateUser_Result
                        {
                            StatusCode = "400",
                            StatusName = "ไม่สำเร็จ",
                            MessageCode = "UMS010_USERNAME_DUPLICATE",
                            MessageName = "ชื่อผู้ใช้งานนี้ถูกใช้ลงทะเบียนแล้ว กรุณาใช้ชื่อผู้ใช้งานอื่น"
                        };
                    }

                    // อีเมลซ้ำอย่างเดียว
                    if (duplicateEmailError != null)
                    {
                        return new UMS010_CreateUser_Result
                        {
                            StatusCode = "400",
                            StatusName = "ไม่สำเร็จ",
                            MessageCode = "UMS010_EMAIL_DUPLICATE",
                            MessageName = "อีเมลนี้ถูกใช้ลงทะเบียนแล้ว กรุณาใช้อีเมลอื่น"
                        };
                    }

                    // เคสอื่น ๆ รวม error message เดิม
                    var errorMessage = string.Join("; ", errors.Select(e => e.Description));
                    return new UMS010_CreateUser_Result
                    {
                        StatusCode = "400",
                        StatusName = "ไม่สำเร็จ",
                        MessageCode = "UMS010_CREATE_FAILED",
                        MessageName = errorMessage
                    };
                }
                var createdByUserNumber = await GetUserNumberByUserNameAsync(criteria.CreateBy);

                var userInfo = new tb_UserInfo
                {
                    Id = appUser.Id,
                    UserNumber = criteria.UserNumber,
                    UserName = criteria.UserName,
                    EmployeeCode = criteria.EmployeeCode,
                    FirstName = criteria.FirstName,
                    LastName = criteria.LastName,
                    DepartmentCode = criteria.DepartmentCode,
                    PositionCode = criteria.PositionCode,
                    Remark = criteria.Remark,
                    LanguageCode = criteria.LanguageCode,
                    CreateDate = DateTime.Now,
                    CreatedBy = criteria.CreateBy,
                };

                _db.UserInfos.Add(userInfo);
                await _db.SaveChangesAsync();
                var rawToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
                var firstLoginUrl = QueryHelpers.AddQueryString(
                    $"/Account/FirstLogin",
                    new Dictionary<string, string?>
                    {
                        ["token"] = encodedToken,
                        ["username"] = appUser.UserName,
                        ["firstname"] = criteria.FirstName,
                        ["lastname"] = criteria.LastName
                    }
                );


                return new UMS010_CreateUser_Result
                {
                    StatusCode = "200",
                    StatusName = "สำเร็จ",
                    MessageCode = "UMS010_CREATE_SUCCESS",
                    MessageName = "สร้างผู้ใช้เรียบร้อยแล้ว",
                    FirstLoginToken = firstLoginUrl
                };
            }
            catch (Exception ex)
            {

                return new UMS010_CreateUser_Result
                {
                    StatusCode = "400",
                    StatusName = "ผิดพลาด",
                    MessageCode = "UMS010_CREATE_EXCEPTION",
                    MessageName = ex.Message
                };
            }

        }

        public async Task<UMS010_UpdateUser_Result> UpdateUser(UMS010_UpdateUser_Criteria criteria)
        {
            try
            {
                var appUser = await _db.ApplicationUsers.FindAsync(criteria.Id);
                if (appUser == null)
                {
                    return new UMS010_UpdateUser_Result
                    {
                        StatusCode = "ERROR",
                        StatusName = "ไม่สำเร็จ",
                        MessageCode = "UMS010_UPDATE_NOT_FOUND",
                        MessageName = "ไม่พบข้อมูลผู้ใช้งาน"
                    };
                }

                var userInfo = await _db.UserInfos.FindAsync(criteria.Id);
                if (userInfo == null)
                {
                    return new UMS010_UpdateUser_Result
                    {
                        StatusCode = "ERROR",
                        StatusName = "ไม่สำเร็จ",
                        MessageCode = "UMS010_UPDATE_USERINFO_NOT_FOUND",
                        MessageName = "ไม่พบข้อมูล User Info"
                    };
                }
                //var updatedByUserNumber = await GetUserNumberByUserNameAsync(criteria.UpdateBy);
                // Update ApplicationUser
                appUser.Email = criteria.Email;
                appUser.NormalizedEmail = criteria.Email?.ToUpperInvariant();
                appUser.PhoneNumber = criteria.PhoneNumer;
                //appUser.FirstLoginFlag = criteria.FirstLoginFlag;
                appUser.ActiveFlag = criteria.ActiveFlag;
                //appUser.SystemAdminFlag = criteria.SystemAdminFlag;

                // Update tb_UserInfo
                //userInfo.UserNumber = criteria.UserNumber;
                userInfo.UserName = criteria.UserName;

                userInfo.FirstName = criteria.FirstName;
                userInfo.LastName = criteria.LastName;
                userInfo.DepartmentCode = criteria.DepartmentCode;
                userInfo.PositionCode = criteria.PositionCode;
                userInfo.Remark = criteria.Remark;
                //userInfo.LanguageCode = criteria.LanguageCode;
                userInfo.UpdateDate = DateTime.Now;
                userInfo.UpdatedBy = criteria.UpdateBy;

                await _db.SaveChangesAsync();


                return new UMS010_UpdateUser_Result
                {
                    StatusCode = "200",
                    StatusName = "สำเร็จ",
                    MessageCode = "UMS010_UPDATE_SUCCESS",
                    MessageName = "อัปเดตผู้ใช้งานเรียบร้อยแล้ว"
                };
            }
            catch (Exception ex)
            {


                return new UMS010_UpdateUser_Result
                {
                    StatusCode = "ERROR",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "UMS010_UPDATE_EXCEPTION",
                    MessageName = ex.Message
                };
            }
        }


        public async Task<UMS010_DeleteUser_Result> DeleteUser(UMS010_DeleteUser_Criteria criteria)
        {
            var result = new UMS010_DeleteUser_Result();



            var user = await _db.UserInfos
                .Where(u => u.Id == criteria.Id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                result.StatusCode = "404";
                result.StatusName = "NotFound";
                result.MessageCode = "USR002";
                result.MessageName = "User not found or already deleted.";
                return result;
            }

            user.DeletedFlag = true;
            user.DeletedBy = criteria.DeletedBy ?? "system";
            user.DeletedDate = DateTime.Now;




            await _db.SaveChangesAsync();
            result.StatusCode = "200";
            result.StatusName = "OK";
            result.MessageCode = "USR000";
            result.MessageName = "User deleted successfully.";

            return result;
        }


        public async Task<UMS010_UpdateUser_Result> AdminChangePassword(UMS010_AdminChangePassword_Criteria criteria)
        {
            var user = await _userManager.FindByNameAsync(criteria.UserName);
            if (user == null)
            {
                return new UMS010_UpdateUser_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "USER_NOT_FOUND",
                    MessageName = "ไม่พบชื่อผู้ใช้งาน"
                };
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, criteria.NewPassword);
            if (!result.Succeeded)
            {
                return new UMS010_UpdateUser_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "CHANGE_PASSWORD_FAILED",
                    MessageName = string.Join("; ", result.Errors.Select(e => e.Description))
                };
            }


            user.LastUpdatePasswordDate = DateTime.Now;
            user.FirstLoginFlag = false;
            await _userManager.UpdateAsync(user);

            return new UMS010_UpdateUser_Result
            {
                StatusCode = "200",
                StatusName = "สำเร็จ",
                MessageCode = "CHANGE_PASSWORD_SUCCESS",
                MessageName = "เปลี่ยนรหัสผ่านเรียบร้อยแล้ว"
            };
        }



        public async Task<UMS010_UpdateUser_Result> ChangePassword(UMS010_ChangePassword_Criteria criteria)
        {
            var user = await _userManager.FindByNameAsync(criteria.UserName);
            if (user == null)
            {
                return new UMS010_UpdateUser_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "USER_NOT_FOUND",
                    MessageName = "ไม่พบชื่อผู้ใช้งาน"
                };
            }


            var passwordValid = await _userManager.CheckPasswordAsync(user, criteria.OldPassword);
            if (!passwordValid)
            {
                return new UMS010_UpdateUser_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "InvalidCurrentPassword",
                    MessageName = "รหัสผ่านเดิมไม่ถูกต้อง"
                };
            }


            var result = await _userManager.ChangePasswordAsync(user, criteria.OldPassword, criteria.NewPassword);
            if (!result.Succeeded)
            {
                return new UMS010_UpdateUser_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "CHANGE_PASSWORD_FAILED",
                    MessageName = string.Join("; ", result.Errors.Select(e => e.Description))
                };
            }


            user.LastUpdatePasswordDate = DateTime.Now;
            user.FirstLoginFlag = false;
            await _userManager.UpdateAsync(user);

            return new UMS010_UpdateUser_Result
            {
                StatusCode = "200",
                StatusName = "สำเร็จ",
                MessageCode = "CHANGE_PASSWORD_SUCCESS",
                MessageName = "เปลี่ยนรหัสผ่านเรียบร้อยแล้ว"
            };
        }


        public async Task<UMS010_FistLogin_Result> FistLogin(UMS010_FistLogin_Criteria criteria)
        {

            var user = await _userManager.FindByNameAsync(criteria.UserName);
            if (user == null)
            {
                return new UMS010_FistLogin_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "USER_NOT_FOUND",
                    MessageName = "ไม่พบชื่อผู้ใช้งาน"
                };
            }

            // 1) ตรวจสอบว่าเป็น first login จริงไหม
            if (user.FirstLoginFlag == false)
            {
                return new UMS010_FistLogin_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "FIRST_LOGIN_ALREADY_DONE",
                    MessageName = "ผู้ใช้งานรายนี้ได้ทำการตั้งรหัสผ่านแล้ว"
                };
            }

            // 2) ตรวจสอบ token
            if (string.IsNullOrEmpty(criteria.Token))
            {
                return new UMS010_FistLogin_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "TOKEN_REQUIRED",
                    MessageName = "ไม่พบข้อมูล Token"
                };
            }

            string rawToken;
            try
            {
                //WebEncoders.Base64UrlDecode(incoming);
                var incoming = (criteria.Token ?? string.Empty).Replace(' ', '+');

                // ถอดแบบ Base64Url ให้กลับเป็น raw token เดิม
                var bytes = WebEncoders.Base64UrlDecode(incoming);
                rawToken = Encoding.UTF8.GetString(bytes);
                // rawToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(criteria.Token));
            }
            catch
            {
                return new UMS010_FistLogin_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "TOKEN_INVALID",
                    MessageName = "Token ไม่ถูกต้อง"
                };
            }

            var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, criteria.OldPassword);
            if (!isOldPasswordValid)
            {
                return new UMS010_FistLogin_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "OLD_PASSWORD_INCORRECT",
                    MessageName = "รหัสผ่านเดิมไม่ถูกต้อง"
                };
            }

            // 3) ตรวจสอบนโยบายรหัสผ่านใหม่
            foreach (var validator in _userManager.PasswordValidators)
            {
                var validate = await validator.ValidateAsync(_userManager, user, criteria.NewPassword);
                if (!validate.Succeeded)
                {
                    return new UMS010_FistLogin_Result
                    {
                        StatusCode = "400",
                        StatusName = "ไม่สำเร็จ",
                        MessageCode = "PASSWORD_POLICY_NOT_MET",
                        MessageName = string.Join("; ", validate.Errors.Select(e => e.Description))
                    };
                }
            }


            var reset = await _userManager.ResetPasswordAsync(user, rawToken, criteria.NewPassword);
            if (!reset.Succeeded)
            {
                return new UMS010_FistLogin_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "RESET_PASSWORD_FAILED",
                    MessageName = string.Join("; ", reset.Errors.Select(e => e.Description))
                };
            }


            user.FirstLoginFlag = false;
            user.LastUpdatePasswordDate = DateTime.Now;


            await _userManager.UpdateSecurityStampAsync(user);
            await _userManager.UpdateAsync(user);

            return new UMS010_FistLogin_Result
            {
                StatusCode = "200",
                StatusName = "สำเร็จ",
                MessageCode = "FIRST_LOGIN_SUCCESS",
                MessageName = "ตั้งรหัสผ่านเรียบร้อยแล้ว"
            };

        }



        public async Task<UMS010_ForgotPassword_Result> ForgotPassword(UMS010_ForgotPassword_Criteria criteria)
        {
            var user = await _userManager.FindByEmailAsync(criteria.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return new UMS010_ForgotPassword_Result
                {
                    StatusCode = "400",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "FORGOT_EMAIL_INVALID",
                    MessageName = "ไม่พบอีเมลหรืออีเมลยังไม่ถูกยืนยัน"
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            //// เช่น https://your-app/reset-password?token={token}&email={email}

            return new UMS010_ForgotPassword_Result
            {
                StatusCode = "200",
                StatusName = "สำเร็จ",
                MessageCode = "FORGOT_PASSWORD_TOKEN",
                MessageName = token,
                Token = token
            };
        }

        public async Task<UMS010_CreateUser_Result> BackOfficeCreateUser(UMS010_CreateUser_Criteria criteria)
        {

            try
            {


                var superAdminCode = "00";
                var superAdminName = "Super Admin";


                var existsPositions = await _db.Positions
                    .AnyAsync(p => p.PositionCode == superAdminCode);

                if (!existsPositions)
                {
                    _db.Positions.Add(new tb_Position
                    {
                        PositionCode = superAdminCode,
                        PositionName = superAdminName,
                    });

                    await _db.SaveChangesAsync();
                }

                superAdminCode = "00";
                superAdminName = "Admin System ";
                var existsDepartments = await _db.Departments
                     .AnyAsync(p => p.DepartmentCode == superAdminCode);

                if (!existsDepartments)
                {
                    _db.Departments.Add(new tb_Department
                    {
                        DepartmentCode = superAdminCode,
                        DepartmentName = superAdminName,
                    });

                    await _db.SaveChangesAsync();
                }

                var GroupAdminName = "Super Admin";
                var GroupAdminDescription = "Full system access";
                var existsRoles = await _db.ApplicationRoles
                     .AnyAsync(p => p.Name == GroupAdminName);




                if (!existsRoles)
                {
                    var role = new ApplicationRole
                    {
                        Name = GroupAdminName,
                        NormalizedName = GroupAdminName.ToUpper(),
                        IsActive = true,
                        Description = GroupAdminDescription
                    };

                    _db.ApplicationRoles.Add(role);
                    await _db.SaveChangesAsync();


                    var insertedGroupId = role.Id;

                    var screenIds = new[] { "UMS010", "UMS020", "UMS030", "CMS040" };
                    foreach (var screenId in screenIds)
                    {
                        var newPermission = new tb_GroupPermission
                        {
                            GroupId = insertedGroupId,
                            ScreenId = screenId,
                            FunctionCode = 63,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            CreateBy = "SYSTEM"
                        };
                        _db.GroupPermissions.Add(newPermission);
                    }
                    await _db.SaveChangesAsync();

                }






                var appUser = new ApplicationUser
                {
                    UserName = criteria.UserName,
                    Email = criteria.Email,
                    PhoneNumber = criteria.PhoneNumer,
                    FirstLoginFlag = true,
                    ActiveFlag = criteria.ActiveFlag,
                    SystemAdminFlag = criteria.SystemAdminFlag
                };

                var result = await _userManager.CreateAsync(appUser, criteria.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.ToList();

                    // เช็คชื่อผู้ใช้งานซ้ำ
                    var duplicateUserNameError = errors
                        .FirstOrDefault(e =>
                            string.Equals(e.Code, "DuplicateUserName", StringComparison.OrdinalIgnoreCase));

                    // เช็คอีเมลซ้ำ
                    var duplicateEmailError = errors
                        .FirstOrDefault(e =>
                            string.Equals(e.Code, "DuplicateEmail", StringComparison.OrdinalIgnoreCase));

                    // ถ้าซ้ำทั้งคู่
                    if (duplicateUserNameError != null && duplicateEmailError != null)
                    {
                        return new UMS010_CreateUser_Result
                        {
                            StatusCode = "400",
                            StatusName = "ไม่สำเร็จ",
                            MessageCode = "UMS010_USERNAME_EMAIL_DUPLICATE",
                            MessageName = "ชื่อผู้ใช้งานและอีเมลนี้ถูกใช้ลงทะเบียนแล้ว กรุณาเปลี่ยนทั้งชื่อผู้ใช้งานและอีเมล"
                        };
                    }

                    // ชื่อผู้ใช้งานซ้ำอย่างเดียว
                    if (duplicateUserNameError != null)
                    {
                        return new UMS010_CreateUser_Result
                        {
                            StatusCode = "400",
                            StatusName = "ไม่สำเร็จ",
                            MessageCode = "UMS010_USERNAME_DUPLICATE",
                            MessageName = "ชื่อผู้ใช้งานนี้ถูกใช้ลงทะเบียนแล้ว กรุณาใช้ชื่อผู้ใช้งานอื่น"
                        };
                    }

                    // อีเมลซ้ำอย่างเดียว
                    if (duplicateEmailError != null)
                    {
                        return new UMS010_CreateUser_Result
                        {
                            StatusCode = "400",
                            StatusName = "ไม่สำเร็จ",
                            MessageCode = "UMS010_EMAIL_DUPLICATE",
                            MessageName = "อีเมลนี้ถูกใช้ลงทะเบียนแล้ว กรุณาใช้อีเมลอื่น"
                        };
                    }

                    // เคสอื่น ๆ รวม error message เดิม
                    var errorMessage = string.Join("; ", errors.Select(e => e.Description));
                    return new UMS010_CreateUser_Result
                    {
                        StatusCode = "ERROR",
                        StatusName = "ไม่สำเร็จ",
                        MessageCode = "UMS010_CREATE_FAILED",
                        MessageName = errorMessage
                    };
                }
                var createdByUserNumber = await GetUserNumberByUserNameAsync(criteria.CreateBy);

                var userInfo = new tb_UserInfo
                {
                    Id = appUser.Id,
                    UserNumber = criteria.UserNumber,
                    UserName = criteria.UserName,
                    EmployeeCode = criteria.EmployeeCode,
                    FirstName = criteria.FirstName,
                    LastName = criteria.LastName,
                    DepartmentCode = criteria.DepartmentCode,
                    PositionCode = criteria.PositionCode,
                    Remark = criteria.Remark,
                    LanguageCode = criteria.LanguageCode,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreatedBy = criteria.CreateBy,
                    UpdatedBy = criteria.CreateBy
                };

                _db.UserInfos.Add(userInfo);
                await _db.SaveChangesAsync();

                var rawToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
                var firstLoginUrl = QueryHelpers.AddQueryString(
                    $"/Account/FirstLogin",
                    new Dictionary<string, string?>
                    {
                        ["token"] = encodedToken,
                        ["username"] = appUser.UserName,
                        ["firstname"] = criteria.FirstName,   // มีได้ แต่ดูหมายเหตุด้านล่าง
                        ["lastname"] = criteria.LastName
                    }
                );


                var CheckRoles = await _db.ApplicationRoles
                     .SingleOrDefaultAsync(p => p.Name == GroupAdminName);
                var exists = await _db.UserRoles
                                .AnyAsync(x => x.UserId == userInfo.Id && x.RoleId == CheckRoles.Id);

                if (!exists)
                {
                    _db.UserRoles.Add(new tb_UserRole
                    {
                        UserId = userInfo.Id,
                        RoleId = CheckRoles.Id
                    });
                }

                await _db.SaveChangesAsync();




                return new UMS010_CreateUser_Result
                {
                    StatusCode = "200",
                    StatusName = "สำเร็จ",
                    MessageCode = "UMS010_CREATE_SUCCESS",
                    MessageName = "สร้างผู้ใช้เรียบร้อยแล้ว",
                    FirstLoginToken = firstLoginUrl
                };
            }
            catch (Exception ex)
            {

                return new UMS010_CreateUser_Result
                {
                    StatusCode = "ERROR",
                    StatusName = "ผิดพลาด",
                    MessageCode = "UMS010_CREATE_EXCEPTION",
                    MessageName = ex.Message
                };
            }
        }

        public async Task<List<SystemConfigs_Result>> SystemConfigs(SystemConfigs_Criteria criteria)
        {



            var result = await _systemDbContext.TsSystemConfigs
                .Where(x => criteria.ConfigCodes.Contains(x.ConfigCode))
                .Select(x => new SystemConfigs_Result
                {
                    ConfigCode = x.ConfigCode,
                    ConfigDesc = x.ConfigDesc,
                    ValueVarchar = x.ValueVarchar,
                    ValueVarchar2 = x.ValueVarchar2,
                    ValueVarchar3 = x.ValueVarchar3,
                    ValueInt = x.ValueInt,
                    ValueDate = x.ValueDate
                })
                .ToListAsync();

            return result;
        }
        public async Task<SendEmailFirstLogin_Result> SendEmailFirstLogin(SendEmailFirstLogin_Criteria criteria)
        {
            try
            {
                var configCodes = new List<string>
                {
                    "PhoneNo",
                    "Software",
                    "Branding",
                    "Email",
                    "Address",
                    "CcToEmail",
                    "URL_Login",
                    "URL_FirstLogin",
                    "EmailCreateUserFirstLogin"

                };

                var systemConfigs = await SystemConfigs(new SystemConfigs_Criteria
                {
                    ConfigCodes = configCodes
                });

                var phoneNo = systemConfigs.FirstOrDefault(x => x.ConfigCode == "PhoneNo")?.ValueVarchar;
                var software = systemConfigs.FirstOrDefault(x => x.ConfigCode == "Software")?.ValueVarchar;
                var branding = systemConfigs.FirstOrDefault(x => x.ConfigCode == "Branding")?.ValueVarchar;
                var email = systemConfigs.FirstOrDefault(x => x.ConfigCode == "Email")?.ValueVarchar;
                var address = systemConfigs.FirstOrDefault(x => x.ConfigCode == "Address")?.ValueVarchar;
                var url_login = systemConfigs.FirstOrDefault(x => x.ConfigCode == "URL_Login")?.ValueVarchar;
                var url_first_login = systemConfigs.FirstOrDefault(x => x.ConfigCode == "URL_FirstLogin")?.ValueVarchar;

                string TemplateEmail = systemConfigs.FirstOrDefault(x => x.ConfigCode == "EmailCreateUserFirstLogin").ValueVarchar;
                string templateSubject = systemConfigs.FirstOrDefault(x => x.ConfigCode == "EmailCreateUserFirstLogin").ValueVarchar2;

                templateSubject = templateSubject
                        .Replace("{BrandName}", branding)
                        .Replace("{SupportEmail}", email)
                        .Replace("{RecipientEmail}", email)
                        .Replace("{SupportPhone}", phoneNo)
                        .Replace("{FirstLoginUrl}", url_first_login + criteria.FirstLoginToken)
                        .Replace("{LoginUrl}", url_login)
                        .Replace("{FullName}", criteria.FullName)
                        .Replace("{Username}", criteria.UserName)
                        .Replace("{Password}", criteria.Password);

                var emailMessage = new EmailMessageDo
                {
                    Subject = TemplateEmail?.Replace("{BrandName}", branding),
                    To = criteria.Email,
                    //CC = systemConfigs.FirstOrDefault(x => x.ConfigCode == "Email")?.ValueVarchar2,
                    Body = templateSubject,
                    //IsHtml = true
                };
                bool result = await _emailService.SendAsync(emailMessage);

                if (result == true)
                {

                    return new SendEmailFirstLogin_Result
                    {
                        StatusCode = "200",
                        StatusName = "Success",
                        MessageCode = "",
                        MessageName = "Send Email Success"
                    };
                }
                return new SendEmailFirstLogin_Result
                {
                    StatusCode = "400",
                    StatusName = "error",
                    MessageCode = "",
                    MessageName = "Send Email error"
                };

            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
