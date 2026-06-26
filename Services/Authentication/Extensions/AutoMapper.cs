using ApplicationDB.Models.System;
using AutoMapper;
using static Authentication.Models.LogImportFileModel;

namespace Authentication.Extensions
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {

            CreateMap<ts_ImportLog, InsertImportLog_Criteria>();
            CreateMap<InsertImportLog_Criteria, ts_ImportLog>();
            CreateMap<ts_ImportLogDetail, ImportLogDetail_Criteria>();
            CreateMap<ImportLogDetail_Criteria, ts_ImportLogDetail>();

        }
    }
}
