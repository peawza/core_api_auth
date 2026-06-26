using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Utils.Extensions
{

    public static class ExceptionHelper
    {
        public static void FailFast(string message)
        {
            Environment.FailFast(message);
        }

        public static string FormatMessage(Exception e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            return string.Format("{0}: {1}", e.GetType().Name, e.Message);
        }

        public static string GetExceptionMessage(Exception ex)
        {
            return GetExceptionMessage(ex, 0);
        }

        public static string GetExceptionMessage(Exception ex, int startingLevel)
        {
            int num = 0;
            StringBuilder sb = new StringBuilder();
            if (num >= startingLevel)
            {
                sb.Append(ex.Message);
                sb.AppendLine();
            }
            if (ex.InnerException != null)
            {
                GetExceptionMessage(ex.InnerException, sb, num + 1, startingLevel);
            }
            return sb.ToString();
        }

        private static void GetExceptionMessage(Exception ex, StringBuilder sb, int currentLevel, int startingLevel)
        {
            if (currentLevel >= startingLevel)
            {
                sb.Append(ex.Message);
                sb.AppendLine();
            }
            if (ex.InnerException != null)
            {
                GetExceptionMessage(ex.InnerException, sb, currentLevel + 1, startingLevel);
            }
        }

        public static Exception GetLastException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }

        public static string GetLastExceptionMessage(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex.Message;
        }

        public static string GetLastExceptionMessage(Exception ex, int iMaxLength)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            string message = ex.Message;
            if (message.Length < iMaxLength)
            {
                return message;
            }
            return message.Substring(0, iMaxLength);
        }

        public static ArgumentException ThrowHelperArgument(string message)
        {
            return (ArgumentException)ThrowHelperError(new ArgumentException(message));
        }

        public static ArgumentException ThrowHelperArgument(string message, Exception innerException)
        {
            return (ArgumentException)ThrowHelperError(new ArgumentException(message, innerException));
        }

        public static ArgumentException ThrowHelperArgument(string paramName, string message)
        {
            return (ArgumentException)ThrowHelperError(new ArgumentException(message, paramName));
        }

        public static Exception ThrowHelperError(Exception exception)
        {
            return exception;
        }
    }
    public class InvalidDataObjectException : Exception
    {
        public InvalidDataObjectException(object data, string message) : base(message)
        {

        }

        public InvalidDataObjectException(object data, string message, string[] validationMessages) : base(message)
        {
            this.ValidationMessages = validationMessages;
        }

        public string[] ValidationMessages { get; private set; }
    }
    public class ServerOperationTypes
    {
        public const string Read = "read";
        public const string Update = "update";
        public const string Create = "create";
        public const string Destroy = "destroy";
    }
    public abstract class AppControllerBase : Controller
    {
        protected IActionResult Ok(string message)
        {
            return base.Ok(new { message });
        }


        protected IActionResult InternalServerError(Exception ex, string operation = ServerOperationTypes.Read)
        {
            var message = ExceptionHelper.GetLastExceptionMessage(ex);
            var invalidError = ex as InvalidDataObjectException;

            var devDetails = new
            {
                exceptionType = ex.GetType().FullName,
                message = ex.Message,
                innerException = ex.InnerException?.Message,
                stackTrace = ex.StackTrace,
                source = ex.Source,
                operation,
            };

            if (invalidError != null)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    resultStatus = false,
                    resultCode = "999",
                    resultMessage = "Validation failed during the operation.",
                    devDetails,
                    validationErrors = invalidError.ValidationMessages
                });
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, new
            {
                resultStatus = false,
                resultCode = "999",
                resultMessage = "The operation encountered an internal server error.",
                devDetails
            });
        }

        protected IActionResult InternalServerError(string message, string operation = ServerOperationTypes.Read)
        {
            // return StatusCode((int)HttpStatusCode.InternalServerError, new { statusCode = (int)HttpStatusCode.InternalServerError, errorCode = HttpStatusCode.InternalServerError.ToString(), message });
            return StatusCode((int)HttpStatusCode.InternalServerError, new { resultStatus = false, resultCode = "999", resultMessage = "The operation was Internal Server Error.", message });

        }

        protected IActionResult InternalServerError(object message)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new { resultStatus = false, resultCode = "999", resultMessage = "The operation was Internal Server Error.", message });
        }


        //BadRequest

        protected IActionResult ServerBadRequest(object data)
        {
            //return StatusCode((int)HttpStatusCode.BadRequest, new { statusCode = (int)HttpStatusCode.BadRequest, errorCode = HttpStatusCode.BadRequest.ToString(), message = data });
            return StatusCode((int)HttpStatusCode.BadRequest, new { resultStatus = false, resultCode = "400", resultMessage = "The operation was BadRequest.", message = data });

        }

        protected IActionResult Ok(object dataObject)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            return Json(new { resultStatus = true, resultCode = "SUCCESS", resultMessage = "The operation was successful.", data = dataObject }, new JsonSerializerSettings() { ContractResolver = contractResolver, Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            //return Json(new { statusCode = (int)HttpStatusCode.OK, data = dataObject });
        }

        protected IActionResult JsonWithCamelNaming(object data)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            return Json(data, new JsonSerializerSettings() { ContractResolver = contractResolver, Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        protected IActionResult JsonWithDefaultOptions(object data, JsonSerializerDefaults options = JsonSerializerDefaults.General)
        {
            //return Json(data, new JsonSerializerOptions(options));
            return Json(data, new JsonSerializerSettings() { Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        protected Dictionary<string, string> GetMappingLocalize<T>(IStringLocalizer localizer)
        {
            var mappings = new Dictionary<string, string>();
            foreach (var p in typeof(T).GetProperties())
            {
                var attr = p.GetCustomAttribute<DisplayAttribute>(true);
                mappings.Add(p.Name, localizer[attr == null ? p.Name : attr.Name].Value);
            }
            return mappings;
        }

        protected string JsonSerializeObject<T>(T data)
        {
            return JsonSerializeObject(data, Formatting.None);
        }

        protected string JsonSerializeObject<T>(T data, Formatting formatting)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                Formatting = formatting,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}
