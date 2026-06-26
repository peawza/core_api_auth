using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Utils.Middleware
{
    public class RequestTimeoutMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TimeSpan _timeout;



        public RequestTimeoutMiddleware(RequestDelegate next, TimeSpan timeout)
        {
            _next = next;
            _timeout = timeout;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var cts = new CancellationTokenSource(_timeout))
            {
                var timeoutTask = Task.Delay(_timeout, cts.Token);
                var nextTask = _next(context);

                var completedTask = await Task.WhenAny(timeoutTask, nextTask);

                if (completedTask == timeoutTask)
                {
                    context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
                    await context.Response.WriteAsync("Request timed out.");
                }
                else
                {
                    cts.Cancel(); // Cancel the timeout task if the request completed in time
                    await nextTask; // Ensure any exceptions/cancellation are observed
                }
            }
        }
    }


    public class CustomErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly IApiLogsService _logger;

        public class CustomErrorResponse
        {
            //public int statusCode { get; set; }
            //public string errorCode { get; set; }
            //public object message { get; set; }

            public bool resultStatus { get; set; }

            public string resultCode { get; set; }
            public string resultMessage { get; set; }
            public object message { get; set; }
        }

        public class OriginalErrorResponse
        {
            public string title { get; set; }
            public object errors { get; set; }
            //public string type { get; set; }

            //public int status { get; set; }
            //public string traceId { get; set; }
        }
        public CustomErrorHandlerMiddleware(RequestDelegate next)
        {

            _next = next;
        }


        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();
            var originalBodyStream = context.Response.Body;

            try
            {
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest)
                    {
                        string requestBody = await ReadRequestBodyAsync(context.Request);
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        var bodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                        context.Response.Body.Seek(0, SeekOrigin.Begin);

                        var originalError = JsonConvert.DeserializeObject<object>(bodyText);

                        var customError = new CustomErrorResponse
                        {
                            resultStatus = false,
                            resultCode = "400",
                            resultMessage = "The operation was Bad Request.",
                            message = originalError

                            //statusCode = (int)HttpStatusCode.BadRequest,
                            //errorCode = HttpStatusCode.BadRequest.ToString(),
                            //message = originalError
                        };

                        var result = JsonConvert.SerializeObject(customError);

                        context.Response.ContentType = "application/json";
                        context.Response.ContentLength = result.Length;
                        context.Response.Body = originalBodyStream;
                        try
                        {
                            //if (originalError.title != "One or more validation errors occurred.")
                            //{
                            //_logger.LogError(context.Request.Path, context.Request.Method, HttpStatusCode.BadRequest, JsonConvert.DeserializeObject<dynamic>(requestBody), JsonConvert.DeserializeObject<dynamic>(result), originalError.errors.ToString());
                            //}
                        }
                        catch (Exception)
                        {


                        }

                        await context.Response.WriteAsync(result);
                    }
                    if (context.Response.StatusCode == (int)HttpStatusCode.InternalServerError)
                    {
                        string requestBody = await ReadRequestBodyAsync(context.Request);
                        //context.Response.Body.Seek(0, SeekOrigin.Begin);
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        var bodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        var originalError = JsonConvert.DeserializeObject<CustomErrorResponse>(bodyText);


                        //_logger.LogError(context.Request.Path, context.Request.Method, HttpStatusCode.InternalServerError, JsonConvert.DeserializeObject<dynamic>(requestBody), JsonConvert.DeserializeObject<dynamic>(bodyText), originalError.message.ToString());




                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                    else if (context.Response.StatusCode == (int)HttpStatusCode.UnsupportedMediaType)
                    {

                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        var bodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                        context.Response.Body.Seek(0, SeekOrigin.Begin);

                        var originalError = JsonConvert.DeserializeObject<OriginalErrorResponse>(bodyText);

                        var customError = new CustomErrorResponse
                        {
                            //statusCode = (int)HttpStatusCode.UnsupportedMediaType,
                            //errorCode = HttpStatusCode.UnsupportedMediaType.ToString(),
                            resultStatus = false,
                            resultCode = "415",
                            resultMessage = "The operation was Unsupported Media Type.",
                            message = originalError
                        };

                        var result = JsonConvert.SerializeObject(customError);

                        context.Response.ContentType = "application/json";
                        context.Response.ContentLength = result.Length;
                        context.Response.Body = originalBodyStream;



                        await context.Response.WriteAsync(result);


                    }
                    //else if (context.Response.StatusCode == (int)HttpStatusCode.OK && context.Response.ContentType != "application/grpc")
                    //{

                    //    string requestBody = await ReadRequestBodyAsync(context.Request);
                    //    //context.Response.Body.Seek(0, SeekOrigin.Begin);
                    //    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    //    var bodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    //    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    //    // var originalError = JsonConvert.DeserializeObject<CustomErrorResponse>(bodyText);


                    //    // _logger.LogError(context.Request.Path, context.Request.Method, HttpStatusCode.InternalServerError, JsonConvert.DeserializeObject<dynamic>(requestBody), JsonConvert.DeserializeObject<dynamic>(bodyText), originalError.message.ToString());
                    //    //_logger.LogInformation(context.Request.Path, context.Request.Method, HttpStatusCode.OK, JsonConvert.DeserializeObject<dynamic>(requestBody), JsonConvert.DeserializeObject<dynamic>(bodyText));



                    //    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    //    await responseBody.CopyToAsync(originalBodyStream);


                    //    /*
                    //     context.Response.Body.Seek(0, SeekOrigin.Begin);





                    //    string requestBody = await ReadRequestBodyAsync(context.Request);

                    //    // Capture the response body
                    //    var originalResponseBodyStream = context.Response.Body;


                    //    using var responseBodyStream = new MemoryStream();
                    //    context.Response.Body = responseBodyStream;

                    //    await _next(context);

                    //    // Read the response body
                    //    string responseBodys = await ReadResponseBodyAsync(responseBodyStream);


                    //    _logger.LogInformation(context.Request.Path, context.Request.Method, HttpStatusCode.OK, JsonConvert.DeserializeObject<dynamic>(requestBody), JsonConvert.DeserializeObject<dynamic>(responseBodys));
                    //    await responseBody.CopyToAsync(originalBodyStream);
                    //       */

                    //    //context.Response.Body.Seek(0, SeekOrigin.Begin);
                    //    //await responseBody.CopyToAsync(originalBodyStream);
                    //}

                    else
                    {

                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
            }
            catch (Exception ex)
            {
                //    _logger.LogError(ex, "An error occurred while processing the request.");
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }

        private async Task<string> ReadResponseBodyAsync(MemoryStream responseBodyStream)
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(responseBodyStream, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            return body;
        }
    }

}