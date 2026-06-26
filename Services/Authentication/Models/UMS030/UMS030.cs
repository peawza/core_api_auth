namespace Authentication.Models.UMS030
{
    public class UMS030
    {

        public partial class UMS030_ListByUserGroup_Criteria
        {
            public string userGroupId { get; set; }

        }

        public partial class GroupPermissionDataView
        {
            public string ModuleCode { get; set; }
            public string ModuleName { get; set; }
            public string SubModuleCode { get; set; }
            public string SubModuleName { get; set; }
            public int Seq { get; set; }
            public string ScreenId { get; set; }
            public string ScreenName { get; set; }
            public int ScreenFunctionCode { get; set; }
            public int PermissionFunctionCode { get; set; }
        }

        public partial class UMS030_UpdatePermission_Criteria
        {
            public string GroupId { get; set; }
            public DateTime? UpdateDate { get; set; }
            public string UpdateBy { get; set; }
            public List<UMS030_UpdatePermission_list_Criteria> GroupPermissionData { get; set; }
        }
        public partial class UMS030_UpdatePermission_list_Criteria
        {
            public string GroupId { get; set; }
            public string ScreenId { get; set; }
            public int FunctionCode { get; set; }
            public DateTime? CreateDate { get; set; }
            public string CreateBy { get; set; }
            public DateTime? UpdateDate { get; set; }
            public string UpdateBy { get; set; }
        }
        public class UMS030_UpdatePermission_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }
        public class UMS030_GetRoles_Criteria
        {
            public string? Id { get; set; }
            public string? RolesName { get; set; }
            public bool? IsActive { get; set; }
        }
        public class UMS030_GetRoles_Result
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public string? NormalizedName { get; set; }
            public string? Description { get; set; }
            public bool? IsActive { get; set; }
        }
        public class UMS030_GetModule_Criteria
        {
            public string? Id { get; set; }
            public string? ModuleCode { get; set; }
            public bool? IsActive { get; set; }
        }
        public class UMS030_GetModule_Result
        {
            public int? Id { get; set; }
            public string? ModuleCode { get; set; }
            public string? ModuleNameEN { get; set; }
            public string? ModuleNameTH { get; set; }
            public int? Seq { get; set; }
            public string? Icon { get; set; }
        }
    }
}
