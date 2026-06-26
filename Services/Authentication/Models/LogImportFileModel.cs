namespace Authentication.Models
{
    public class LogImportFileModel
    {
        public class API_InsertImportLog_Criteria
        {
            public InsertImportLog_Criteria ImportLog { get; set; }


            public List<ImportLogDetail_Criteria>? ImportLogDetail { get; set; }
        }


        #region InsertImportLog


        public class InsertImportLog_Criteria
        {


            public string JobLogId { get; set; }

            public string InterfaceCode { get; set; }

            public string InterfaceName { get; set; }

            public string FtpServerName { get; set; }

            public string FileFolder { get; set; }

            public string InterfaceFileName { get; set; }

            public string JobStatus { get; set; }

            public string ProcessBy { get; set; }

            public DateTime? StartDateTime { get; set; }

            public DateTime? FinishDateTime { get; set; }

            public int TotalRecord { get; set; }

            public string Remark { get; set; }
        }

        public class InsertImportLog_Result
        {


        }
        #endregion


        #region ImportLogDetail


        public class ImportLogDetail_Criteria
        {
            public Guid Id { get; set; }

            public string? JobLogId { get; set; }

            public int? RowNo { get; set; }

            public string? ErrorDetail { get; set; }
        }

        public class ImportLogDetail_Result
        {


        }
        #endregion
    }
}
