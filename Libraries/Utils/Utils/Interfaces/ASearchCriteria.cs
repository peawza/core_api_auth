namespace Utils.Interfaces
{
    public abstract class ASearchCriteriaDo
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public int Skip
        {
            get
            {
                return this.PageIndex * this.PageSize;
            }
        }
        public int Take
        {
            get
            {
                return this.PageSize;
            }
        }
    }
    public abstract class ASearchResultDo<T> where T : class
    {
        public int TotalRecords { get; set; }
        public List<T> Rows { get; set; }

        public ASearchResultDo()
        {
            this.Rows = new List<T>();
        }
    }
}
