using System.ComponentModel.DataAnnotations;

namespace Authentication.Models
{
    public class MESScreenModel
    {

        #region LocalizedMessages
        public class MESScreenCriteria
        {
            [Required]
            public string? SupportDeviceType { get; set; }
        }


        public class MESScreenResult
        {
            public string ScreenId { get; set; }
            public string Name_EN { get; set; }
            public string Name_TH { get; set; }
            public int FunctionCode { get; set; }
            public string ModuleCode { get; set; }
            public int ModuleSeq { get; set; }
            public int ModuleName_Seq { get; set; }
            public string ModuleName_EN { get; set; }
            public string ModuleName_TH { get; set; }
            public string? ModuleName_IconClass { get; set; }
            public string SubModuleCode { get; set; }
            public int SubModuleSeq { get; set; }
            public string SubModuleName_EN { get; set; }
            public string SubModuleName_TH { get; set; }
            public string SubModule_IconClass { get; set; }
            public string Screen_IconClass { get; set; }
            public bool? Screen_MainMenuFlag { get; set; }
            public bool? Screen_PermissionFlag { get; set; }
            public int Screen_Seq { get; set; }
            public string PageTitleName_EN { get; set; }
            public string PageTitleName_TH { get; set; }

        }





        #endregion


        #region GetSiteMaps
        public class GetSiteMaps_Criteria
        {
            [Required]
            public string? SupportDeviceType { get; set; }
        }


        public class GetSiteMaps_Result
        {
            public List<ModuleModel> Module { get; set; }
            public List<SubModuleModel> SubModule { get; set; }
            public List<GetSiteScreenModel> Screen { get; set; }

        }

        public class ModuleModel
        {
            public string ModuleCode { get; set; }
            public string Module_EN { get; set; }
            public string Module_TH { get; set; }
            public int Seq { get; set; }

            public string IconClass { get; set; }
        }

        public class SubModuleModel
        {
            public string SubModuleCode { get; set; }
            public string SubModuleName_EN { get; set; }
            public string SubModuleName_TH { get; set; }
            public int Seq { get; set; }

            public string IconClass { get; set; }
        }
        public class GetSiteScreenModel
        {
            public string ScreenId { get; set; }
            public string Name_EN { get; set; }
            public string Name_TH { get; set; }
            public int FunctionCode { get; set; }
            public string ModuleCode { get; set; }
            public int ModuleSeq { get; set; }
            public string ModuleName_EN { get; set; }
            public string ModuleName_TH { get; set; }
            public int ModuleName_Seq { get; set; }
            public string? ModuleName_IconClass { get; set; }
            public string SubModuleCode { get; set; }
            public int SubModuleSeq { get; set; }
            public string SubModuleName_EN { get; set; }
            public string SubModuleName_TH { get; set; }
            public string SubModule_IconClass { get; set; }
            public string Screen_IconClass { get; set; }
            public bool? Screen_MainMenuFlag { get; set; }
            public bool? Screen_PermissionFlag { get; set; }
            public int Screen_Seq { get; set; }
            public string PageTitleName_EN { get; set; }
            public string PageTitleName_TH { get; set; }


        }

        #endregion


    }
}
