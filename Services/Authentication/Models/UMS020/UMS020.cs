namespace Authentication.Models.UMS020
{
    public class UMS020
    {
        #region MS020_SearchRole
        public partial class UMS020_SearchRole_Criteria
        {

            public string? GroupName { get; set; }
            public string? GroupDescription { get; set; }
            public string? UserName { get; set; }
            public bool? Status { get; set; }

        }
        public class MS020_SearchRole_Result
        {
            public string Id { get; set; }
            public string? Name { get; set; }
            public string? NormalizedName { get; set; }
            public string? Description { get; set; }
            public bool IsActive { get; set; }
            public string? CreateBy { get; set; }
            public DateTime? CreateDate { get; set; }
            public string? UpdateBy { get; set; }
            public DateTime? UpdateDate { get; set; }
            public List<CMS020_SearchDetail_Result>? Detail { get; set; }

        }

        public class CMS020_SearchDetail_Result
        {
            public string? UserId { get; set; }
            public string? Username { get; set; }
            public string? name { get; set; }
            public string? DepartmentCode { get; set; }
            public string? DisplayDepartmentCode { get; set; }
            public string? PositionCode { get; set; }
            public string? DisplayPosition { get; set; }
        }

        #endregion


        #region UMS020_AddRole
        public partial class UMS020_AddRole_Criteria
        {

            //
            public string? Name { get; set; }
            public string? NormalizedName { get; set; }
            public string? Description { get; set; }
            public bool? IsActive { get; set; }
            public string? CreateBy { get; set; }
            public DateTime CreateDate { get; set; } = DateTime.Now;

        }
        public class UMS020_AddRole_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }
        #endregion


        #region UMS020_UpdateRole
        public partial class UMS020_UpdateRole_Criteria : UMS020_AddRole_Criteria
        {
            public string Id { get; set; }
            public string? UpdateBy { get; set; }
            public DateTime UpdateDate { get; set; } = DateTime.Now;

        }
        public class UMS020_UpdateRole_Result : UMS020_AddRole_Result
        {

        }
        #endregion

        #region UMS020_DeleteRole
        public partial class UMS020_DeleteRole_Criteria : UMS020_AddRole_Criteria
        {
            public string Id { get; set; }

        }
        public class UMS020_DeleteRole_Result : UMS020_AddRole_Result
        {

        }
        #endregion

        #region UMS020_UserRoleMapping
        public partial class UMS020_UserRoleMapping_Criteria
        {
            public string RoleId { get; set; }
            public string[]? Users { get; set; }
            public string? User { get; set; }
            public string? jsonListUser { get; set; }
        }
        public class UMS020_UserRoleMapping_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }
        #endregion

        #region UMS020_GetUserRoleMapping
        public class UMS020_GetUserRoleMapping_Criteria
        {
            public string? RoleId { get; set; }
        }
        public class UMS020_GetUserRoleMapping_Result
        {
            public List<UMS020_UserList_Result>? UserListOutRole { get; set; }
            public List<UMS020_UserList_Result>? UserListInRole { get; set; }
        }
        public class UMS020_UserList_Result
        {
            public string? UserId { get; set; }
            public string? UserName { get; set; }
            public string? DisplayFullName { get; set; }
            public string? DisplayDepartment { get; set; }
        }
        #endregion
    }
}
