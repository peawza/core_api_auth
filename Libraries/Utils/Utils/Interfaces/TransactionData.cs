namespace Utils.Interfaces
{
    public abstract class AUpdateTransactionDo
    {
        public List<string> Errors { get; set; }

        public AUpdateTransactionDo()
        {
            this.Errors = new List<string>();
        }

        public void AddError(params string[] errors)
        {
            if (errors != null)
            {
                foreach (string error in errors)
                {
                    if (error != null)
                        this.Errors.Add(error);
                }
            }

        }
        public bool HasError
        {
            get
            {
                return this.Errors.Count > 0;
            }
        }
    }
    public abstract class AUpdateTransactionDo<T> : AUpdateTransactionDo where T : class
    {
        public T? Data { get; set; }

        public AUpdateTransactionDo() : base()
        {
        }
    }
}
