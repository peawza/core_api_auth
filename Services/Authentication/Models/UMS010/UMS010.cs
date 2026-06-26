using System.ComponentModel.DataAnnotations;

namespace Authentication.Models.NewFolder
{
    public class UMS010
    {

        #region MyRegion
        public class UMS010_Search_Criteria
        {
            public string? UserName { get; set; }
            public string? Name { get; set; }
            public string? PermissionGroup { get; set; }
            public string? Department { get; set; }
            public bool? ActiveFlag { get; set; }
        }

        public class UMS010_Search_Result
        {
            public string Id { get; set; }
            public int UserNumber { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string DisplayName { get; set; }
            public string? DepartmentCode { get; set; }
            public string? DisplayDepartment { get; set; }
            public string? PositionCode { get; set; }
            public string? DisplayPosition { get; set; }
            public int? PermissionGroupId { get; set; }
            public string? DisplayPermissionGroup { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public bool? IsActive { get; set; }
            public string? Remark { get; set; }
            public string? LanguageCode { get; set; }
            public DateTime CreateDate { get; set; }
            public string CreateBy { get; set; }
            public DateTime? UpdateDate { get; set; }
            public string UpdateBy { get; set; }

            public DateTime? LastLoginDate { get; set; }
            public DateTime? LastChangedPassword { get; set; }
        }


        #endregion


        #region
        public class GetUserID_Info_Criteria
        {
            public string UserId { get; set; }
        }

        public class GetUserID_Info_Result
        {
            public string Id { get; set; }
            public int UserNumber { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string? DepartmentCode { get; set; }
            public string? PositionCode { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Remark { get; set; }
            public string? LanguageCode { get; set; }
            public DateTime CreateDate { get; set; }
            public string CreateBy { get; set; }
            public DateTime? UpdateDate { get; set; }
            public string UpdateBy { get; set; }
            public bool FirstLoginFlag { get; set; }
            public bool ActiveFlag { get; set; }
            public bool SystemAdminFlag { get; set; }
            public int? PasswordAge { get; set; }
            public DateTime? LastLoginDate { get; set; }
            public DateTime? LastUpdatePasswordDate { get; set; }
            public DateTime? ActiveDate { get; set; }
            public DateTime? InActiveDate { get; set; }
        }
        #endregion


        #region UMS010_CreateUser_Criteria
        public class UMS010_CreateUser_Criteria
        {
            // ApplicationUser properties
            public string UserName { get; set; }
            public string Email { get; set; }
            public string? PhoneNumer { get; set; }
            public bool FirstLoginFlag { get; set; }
            public bool ActiveFlag { get; set; }
            public bool SystemAdminFlag { get; set; }

            public string? Password { get; set; }  // plaintext password

            // tb_UserInfo properties
            public int UserNumber { get; set; }
            public string? EmployeeCode { get; set; }
            public string? DepartmentCode { get; set; }
            public string? PositionCode { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string? Remark { get; set; }
            public string? LanguageCode { get; set; }
            public string? CreateBy { get; set; }
        }

        public class UMS010_CreateUser_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
            public int UserId { get; set; }
            public string Username { get; set; }
            public string FirstLoginToken { get; set; }

        }

        #endregion


        #region UMS010_UpdateUser
        public class UMS010_UpdateUser_Criteria
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string? PhoneNumer { get; set; }
            public bool FirstLoginFlag { get; set; }
            public bool ActiveFlag { get; set; }
            public bool SystemAdminFlag { get; set; }
            public int UserNumber { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string? DepartmentCode { get; set; }
            public string? PositionCode { get; set; }
            public string? Remark { get; set; }
            public string? LanguageCode { get; set; }

            public string? UpdateBy { get; set; }
        }
        public class UMS010_UpdateUser_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
            public string UserName { get; set; }
        }
        #endregion
        #region UMS010_DeleteUser

        public class UMS010_DeleteUser_Criteria
        {
            public string Id { get; set; }
            public string? DeletedBy { get; set; }

        }
        public class UMS010_DeleteUser_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
            public string? UserName { get; set; }
        }
        #endregion


        #region UMS010_AdminChangePassword

        public class UMS010_AdminChangePassword_Criteria
        {
            public string UserName { get; set; }
            public string NewPassword { get; set; }
        }
        public class UMS010_AdminChangePassword_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }

        #endregion


        #region UMS010_ChangePassword
        public class UMS010_ChangePassword_Criteria
        {
            public string UserName { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
        public class UMS010_ChangePassword_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }

        #endregion


        #region UMS010_FistLogin
        public class UMS010_FistLogin_Criteria
        {
            [Required]
            public string? UserName { get; set; }
            [Required]
            public string? OldPassword { get; set; }
            [Required]
            public string? NewPassword { get; set; }
            [Required]
            public string? Token { get; set; }

        }
        public class UMS010_FistLogin_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }

        #endregion

        #region ForgotPassword
        public class UMS010_ForgotPassword_Criteria
        {
            public string Email { get; set; }
        }
        public class UMS010_ForgotPassword_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
            public string? Token { get; set; }
        }

        #endregion




        //UMS010_CreateUser_Criteria
        #region API_UpdateUserMes_Criteria
        public class UMS010_CreateUserMes_Criteria : UMS010_CreateUser_Criteria
        {


        }

        public class UMS010_CreateUserMes_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }


        #endregion
        #region API_UpdateUserMes_Criteria
        public class UMS010_UpdateUserMes_Criteria
        {
            // ApplicationUser properties
            public string UserName { get; set; }
            public string Firstname { get; set; } = null!;
            public string? Lastname { get; set; }
            public string Email { get; set; } = null!;
            public bool Status { get; set; }
            public string? PhoneNo { get; set; }
            public string? UpdatedBy { get; set; }

        }

        public class UMS010_UpdateUserMes_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }

        #endregion


        #region UMS010_DeleteUserMes_Criteria
        public class UMS010_DeleteUserMes_Criteria
        {
            public string UserName { get; set; }
            public string? DeletedBy { get; set; }

        }

        public class UMS010_DeleteUserMes_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }

        #endregion


        #region SendEmailFirstLogin
        public class SendEmailFirstLogin_Criteria
        {
            public string FullName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public string FirstLoginToken { get; set; }

        }
        public class SendEmailFirstLogin_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }


        #endregion

        #region SystemConfigs


        public class SystemConfigs_Criteria
        {
            public List<string> ConfigCodes { get; set; } = new();

        }

        public class SystemConfigs_Result
        {
            public string ConfigCode { get; set; } = null!;

            public string? ConfigDesc { get; set; }

            public string? ValueVarchar { get; set; }

            public string? ValueVarchar2 { get; set; }

            public string? ValueVarchar3 { get; set; }

            public int? ValueInt { get; set; }

            public DateTime? ValueDate { get; set; }

        }

        #endregion
    }
}
