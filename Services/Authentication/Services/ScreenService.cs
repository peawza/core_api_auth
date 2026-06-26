using Authentication.Repositories;
using static Authentication.Models.LogImportFileModel;
using static Authentication.Models.MESScreenModel;

namespace Authentication.Services
{
    public partial interface IScreenService
    {
        Task<List<MESScreenResult>>? getMesScreen(MESScreenCriteria criteria);

        Task<GetSiteMaps_Result>? GetSiteMaps(GetSiteMaps_Criteria criteria);

        Task<string> InsertImportLog(InsertImportLog_Criteria importlog_criteria, List<ImportLogDetail_Criteria>? importlogdetail_criteria);
    }

    public partial class ScreenService : IScreenService
    {
        private readonly IScreenRepository repository;

        private readonly ILogImportFileRepository _logImportFile_repository;

        public ScreenService(
            IScreenRepository repository,
            ILogImportFileRepository logImportFile_repository
        )
        {
            this.repository = repository;

            _logImportFile_repository = logImportFile_repository;
        }


        public async Task<List<MESScreenResult>>? getMesScreen(MESScreenCriteria criteria)
        {
            try
            {
                return await this.repository.getMesScreen(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<GetSiteMaps_Result>? GetSiteMaps(GetSiteMaps_Criteria criteria)
        {
            try
            {
                return await this.repository.GetSiteMaps(criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> InsertImportLog(InsertImportLog_Criteria importlog_criteria, List<ImportLogDetail_Criteria>? importlogdetail_criteria)
        {
            try
            {
                string JobLogId = Guid.NewGuid().ToString();
                string JobStatus = importlogdetail_criteria.Count != 0 ? "E" : "S";

                importlog_criteria.JobLogId = JobLogId;
                importlog_criteria.JobStatus = JobStatus;
                await _logImportFile_repository.InsertImportLog(importlog_criteria);


                if (importlogdetail_criteria.Count != 0)
                {
                    for (int i = 0; i < importlogdetail_criteria.Count; i++)
                    {
                        var date_index = importlogdetail_criteria[i];

                        date_index.Id = Guid.NewGuid();
                        date_index.JobLogId = JobLogId;
                        await _logImportFile_repository.InsertImportLogDetail(date_index);

                    }
                }

                return JobLogId;
            }
            catch (Exception)
            {

                throw;
            }

        }


    }
}
