using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Utils.Web.Services
{
    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec says 70 characters is a reasonable limit.
        public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary);
            if (string.IsNullOrWhiteSpace(boundary.Value))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary.Value;
        }

        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="key";
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                   && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
        }

        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                       || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }
    }

    public interface IFileUpload
    {
        Task<Models.UploadObjectDo<T>> ToFile<T>(HttpRequest request, string suffixFileName = null, bool asFile = true) where T : class;
    }
    public class FileUpload : IFileUpload
    {
        // Get the default form options so that we can use them to set the default limits for
        // request body data
        private static readonly Microsoft.AspNetCore.Http.Features.FormOptions defaultFormOptions = new Microsoft.AspNetCore.Http.Features.FormOptions();


        public FileUpload()
        {
        }

        public async Task<Models.UploadObjectDo<T>> ToFile<T>(HttpRequest request, string suffixFileName = null, bool asFile = true) where T : class
        {
            if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType))
                return null;

            Models.UploadObjectDo<T> obj = new Models.UploadObjectDo<T>();

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(request.ContentType),
                defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {

                        string fileName = contentDisposition.FileName.Value.Replace("\"", "");

                        if (asFile == true)
                        {
                            string extension = System.IO.Path.GetExtension(fileName);
                            fileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                            if (string.IsNullOrWhiteSpace(suffixFileName) == false)
                                fileName = string.Format("{0}.{1}", fileName, suffixFileName);
                            //fileName = fileName + extension;
                            //Add By Narut T.  Fixed upload same file name at same time
                            int iCount = 1;
                            string newFileName = fileName + extension;
                            while (obj.Files.Find(f => f.FileName == newFileName) != null)
                            {
                                newFileName = string.Format("{0}({1}){2}", fileName, iCount, extension);
                                iCount = iCount + 1;
                            }
                            fileName = newFileName;
                            //End Add

                            string targetFilePath = System.IO.Path.Combine(Utils.Constants.COMMON.TEMP_PATH, fileName);
                            using (var targetStream = System.IO.File.Create(targetFilePath))
                            {
                                await section.Body.CopyToAsync(targetStream);
                            }

                            System.IO.FileInfo f = new FileInfo(targetFilePath);
                            if (f.Exists)
                            {
                                Models.FileUploadDo file = new Models.FileUploadDo();
                                file.FileName = f.Name;
                                file.FilePath = f.FullName;

                                obj.Files.Add(file);
                            }
                        }
                        else
                        {
                            Models.FileUploadDo file = new Models.FileUploadDo();
                            file.FileName = System.IO.Path.GetFileName(contentDisposition.FileName.Value.Replace("\"", ""));

                            using (MemoryStream s = new MemoryStream())
                            {
                                await section.Body.CopyToAsync(s);
                                s.Position = 0;

                                byte[] b = new byte[s.Length];
                                s.Read(b, 0, (int)s.Length);

                                if (file.BinaryFile == null)
                                    file.BinaryFile = b;
                            }

                            obj.Files.Add(file);
                        }
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Do not limit the key name length here because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }

                            obj.Parameter = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            return obj;
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableFormValueModelBindingAttribute : Attribute, Microsoft.AspNetCore.Mvc.Filters.IResourceFilter
    {
        public void OnResourceExecuting(Microsoft.AspNetCore.Mvc.Filters.ResourceExecutingContext context)
        {
            var formValueProviderFactory = context.ValueProviderFactories
                .OfType<Microsoft.AspNetCore.Mvc.ModelBinding.FormValueProviderFactory>()
                .FirstOrDefault();
            if (formValueProviderFactory != null)
            {
                context.ValueProviderFactories.Remove(formValueProviderFactory);
            }

            var jqueryFormValueProviderFactory = context.ValueProviderFactories
                .OfType<Microsoft.AspNetCore.Mvc.ModelBinding.JQueryFormValueProviderFactory>()
                .FirstOrDefault();
            if (jqueryFormValueProviderFactory != null)
            {
                context.ValueProviderFactories.Remove(jqueryFormValueProviderFactory);
            }
        }

        public void OnResourceExecuted(Microsoft.AspNetCore.Mvc.Filters.ResourceExecutedContext context)
        {
        }
    }
}