using System.ComponentModel.DataAnnotations;

namespace Authentication.Models.CMS040
{
    public class CMS040
    {
        #region CMS040_SearchMiscellaneous

        public partial class CMS040_SearchMiscellaneous_Criteria
        {
            public string? MiscValue { get; set; }
            public string? MiscType { get; set; }
            public bool? Status { get; set; }
        }
        public partial class CMS040_SearchMiscellaneous_Result
        {
            public string MiscTypeCode { get; set; } = null!;
            public string? MiscTypeName { get; set; }
            public int? SeqNo { get; set; }
            public string MiscCode { get; set; } = null!;
            public string? MiscName { get; set; }     // มาจาก Description ใน Value table
            public string? Value1 { get; set; }
            public string? Value2 { get; set; }
            public string? Description { get; set; }
            public bool Status { get; set; }
            public string? TextStatus { get; set; }
            public string? CreateBy { get; set; }
            public DateTime? CreateDate { get; set; }
            public string? UpdateBy { get; set; }
            public DateTime? UpdateDate { get; set; }

        }
        #endregion

        #region CMS040_MiscellaneousDetail
        public class CMS040_MiscellaneousDetail_Criteria
        {
            [Required]
            public string MiscTypeCode { get; set; } = null!;
            [Required]
            public string MiscCode { get; set; } = null!;
        }

        public class CMS040_MiscellaneousDetail_Result
        {
            public int MiscCodeId { get; set; }
            public string MiscTypeCode { get; set; } = null!;
            public string MiscCode { get; set; } = null!;
            public string? Value1 { get; set; }
            public string? Value2 { get; set; }
            public string? Description { get; set; }
            public int SeqNo { get; set; }
            public bool ActiveFlag { get; set; }
        }
        #endregion

        #region CMS040_DeleteMiscellaneous

        public partial class CMS040_DeleteMiscellaneous_Criteria
        {
            public string MiscTypeCode { get; set; } = null!;
            public string MiscCode { get; set; } = null!;
            public string? DeletedBy { get; set; }
        }
        public partial class CMS040_DeleteMiscellaneous_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }
        #endregion

        #region CMS040_InsertMiscellaneous

        public partial class CMS040_InsertMiscellaneous_Criteria
        {
            public string? MiscTypeCode { get; set; }
            public string? MiscCode { get; set; }
            public string? MiscName { get; set; }
            public string? Value1 { get; set; }
            public string? Value2 { get; set; }
            public int SeqNo { get; set; }
            public string? Description { get; set; }
            public bool Status { get; set; }
            public string CreatedBy { get; set; } = null!;
        }
        public partial class CMS040_InsertMiscellaneous_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }
        #endregion

        #region CMS040_UpdateMiscellaneous

        public partial class CMS040_UpdateMiscellaneous_Criteria : CMS040_InsertMiscellaneous_Criteria
        {

            public string UpdatedBy { get; set; } = null!;
        }
        public partial class CMS040_UpdateMiscellaneous_Result
        {
            public string? StatusCode { get; set; }
            public string? StatusName { get; set; }
            public string? MessageCode { get; set; }
            public string? MessageName { get; set; }
        }
        #endregion

        #region MyRegion
        public class CMS040_MiscellaneousType_Criteria
        {

        }
        public class CMS040_MiscellaneousType_Result
        {
            public int MiscTypeId { get; set; }
            public string MiscTypeCode { get; set; } = null!;
            public string? MiscTypeName { get; set; }
        }



        #endregion
    }
}
