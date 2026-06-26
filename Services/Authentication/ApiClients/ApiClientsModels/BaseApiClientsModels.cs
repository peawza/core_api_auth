namespace Authentication.ApiClients.ApiClientsModels
{
    public class BaseApiClientsModels
    {

        public class ApiResponse<T>
        {
            public bool ResultStatus { get; set; }
            public string? ResultCode { get; set; }
            public string? ResultMessage { get; set; }
            public T? Data { get; set; }
        }
    }
}
