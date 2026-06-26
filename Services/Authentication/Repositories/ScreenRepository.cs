using Application;
using Microsoft.EntityFrameworkCore;
using static Authentication.Models.MESScreenModel;

namespace Authentication.Repositories
{
    public interface IScreenRepository
    {

        Task<List<MESScreenResult>>? getMesScreen(MESScreenCriteria criteria);

        Task<GetSiteMaps_Result>? GetSiteMaps(GetSiteMaps_Criteria criteria);
    }

    public class ScreenRepository : IScreenRepository, IDisposable
    {
        private SystemDbContext db { get; set; }

        public ScreenRepository(SystemDbContext db)
        {
            this.db = db;
        }




        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<List<MESScreenResult>>? getMesScreen(MESScreenCriteria criteria)
        {
            try
            {

                List<MESScreenResult> result = await (from screen in db.Screen
                                                      join module in db.Module on screen.ModuleCode equals module.ModuleCode
                                                      //join subModule in db.SubModule on screen.SubModuleCode equals subModule.SubModuleCode
                                                      join subModule in db.SubModule on screen.SubModuleCode equals subModule.SubModuleCode into subModuleJoin
                                                      from subModule in subModuleJoin.DefaultIfEmpty()
                                                      where screen.SupportDeviceType.Contains(criteria.SupportDeviceType)
                                                      orderby module.Seq, subModule.Seq, screen.Seq
                                                      select new MESScreenResult
                                                      {
                                                          ScreenId = screen.ScreenId,
                                                          Name_EN = screen.NameEN,
                                                          Name_TH = screen.NameTH,
                                                          FunctionCode = screen.FunctionCode,
                                                          ModuleCode = module.ModuleCode,
                                                          ModuleSeq = module.Seq,
                                                          ModuleName_EN = module.ModuleNameEN,
                                                          ModuleName_TH = module.ModuleNameTH,
                                                          ModuleName_Seq = module.Seq,
                                                          ModuleName_IconClass = module.IconClass,

                                                          SubModuleCode = subModule != null ? subModule.SubModuleCode : null,
                                                          SubModuleName_EN = subModule != null ? subModule.SubModuleNameEN : null,
                                                          SubModuleName_TH = subModule != null ? subModule.SubModuleNameTH : null,
                                                          SubModuleSeq = subModule != null ? subModule.Seq : (int)0,
                                                          SubModule_IconClass = subModule != null ? subModule.IconClass : null,

                                                          Screen_IconClass = screen.IconClass,
                                                          Screen_MainMenuFlag = screen.MainMenuFlag,
                                                          Screen_PermissionFlag = screen.PermissionFlag,
                                                          Screen_Seq = screen.Seq,
                                                          PageTitleName_EN = screen.PageTitleNameEN,
                                                          PageTitleName_TH = screen.PageTitleNameTH,
                                                      }).ToListAsync();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<GetSiteMaps_Result>? GetSiteMaps(GetSiteMaps_Criteria criteria)
        {
            GetSiteMaps_Result messcreenResult = new GetSiteMaps_Result();
            try
            {


                List<ModuleModel> ModuleList = await (from module in db.Module
                                                      select new ModuleModel
                                                      {

                                                          ModuleCode = module.ModuleCode,
                                                          Module_EN = module.ModuleNameEN,
                                                          Module_TH = module.ModuleNameTH,
                                                          Seq = module.Seq,
                                                          IconClass = module.IconClass,

                                                      }).ToListAsync();
                List<SubModuleModel> SubModule = await (from module in db.SubModule
                                                        select new SubModuleModel
                                                        {

                                                            SubModuleCode = module.SubModuleCode,
                                                            SubModuleName_EN = module.SubModuleNameEN,
                                                            SubModuleName_TH = module.SubModuleNameTH,
                                                            Seq = module.Seq,
                                                            IconClass = module.IconClass,

                                                        }).ToListAsync();


                List<GetSiteScreenModel> SiteScreen = await (from screen in db.Screen
                                                             join module in db.Module on screen.ModuleCode equals module.ModuleCode
                                                             join subModule in db.SubModule on screen.SubModuleCode equals subModule.SubModuleCode into subModuleJoin
                                                             from subModule in subModuleJoin.DefaultIfEmpty()
                                                             where screen.SupportDeviceType.Contains(criteria.SupportDeviceType)
                                                             orderby module.Seq, subModule.Seq, screen.Seq
                                                             select new GetSiteScreenModel
                                                             {
                                                                 ScreenId = screen.ScreenId,
                                                                 Name_EN = screen.NameEN,
                                                                 Name_TH = screen.NameTH,
                                                                 FunctionCode = screen.FunctionCode,
                                                                 ModuleCode = module.ModuleCode,
                                                                 ModuleSeq = module.Seq,
                                                                 ModuleName_EN = module.ModuleNameEN,
                                                                 ModuleName_TH = module.ModuleNameTH,
                                                                 ModuleName_Seq = module.Seq,
                                                                 ModuleName_IconClass = module.IconClass,
                                                                 SubModuleCode = subModule != null ? subModule.SubModuleCode : null,
                                                                 SubModuleName_EN = subModule != null ? subModule.SubModuleNameEN : null,
                                                                 SubModuleName_TH = subModule != null ? subModule.SubModuleNameTH : null,
                                                                 SubModuleSeq = subModule != null ? subModule.Seq : (int)0,
                                                                 SubModule_IconClass = subModule != null ? subModule.IconClass : null,
                                                                 Screen_IconClass = screen.IconClass,
                                                                 Screen_MainMenuFlag = screen.MainMenuFlag,
                                                                 Screen_PermissionFlag = screen.PermissionFlag,
                                                                 Screen_Seq = screen.Seq,
                                                                 PageTitleName_EN = screen.PageTitleNameEN,
                                                                 PageTitleName_TH = screen.PageTitleNameTH,
                                                             }).ToListAsync();

                messcreenResult.Module = ModuleList;
                messcreenResult.SubModule = SubModule;
                messcreenResult.Screen = SiteScreen;

                return messcreenResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
