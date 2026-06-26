using Application;
using ApplicationDB.Models.System;
using AutoMapper;

using static Authentication.Models.LogImportFileModel;

namespace Authentication.Repositories
{

    public interface ILogImportFileRepository
    {

        Task InsertImportLog(InsertImportLog_Criteria criteria);
        Task InsertImportLogDetail(ImportLogDetail_Criteria criteria);

    }
    public class LogImportFileRepository : ILogImportFileRepository
    {

        private readonly SystemDbContext _db;
        private readonly IMapper _mapper;

        public LogImportFileRepository(SystemDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task InsertImportLog(InsertImportLog_Criteria criteria)
        {
            try
            {
                var model = _mapper.Map<ts_ImportLog>(criteria);
                _db.tsImportLogs.Add(model);
                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public async Task InsertImportLogDetail(ImportLogDetail_Criteria criteria)
        {
            try
            {
                var model = _mapper.Map<ts_ImportLogDetail>(criteria);
                _db.tsImportLogDetails.Add(model);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }

        }


    }
}
