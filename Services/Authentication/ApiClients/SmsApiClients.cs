using Application;
using ApplicationDB.Models.System;
using Authentication.Repositories;
using System.Text;
using System.Text.Json;
using static Authentication.ApiClients.ApiClientsModels.SmsSendClientsModels;
using static Authentication.Models.CommonModels;

namespace Authentication.ApiClients
{

    public partial interface ISmsApiClients
    {
        Task<SmsSendHtml_Result> SendSmsAsync(SmsSendHtml_Criteria criteria);
    }

    public class SmsApiClients : ISmsApiClients
    {
        private readonly HttpClient _httpClient;
        private readonly ICommonRepository _commonRepository;
        private readonly SystemDbContext _systemDb;



        public SmsApiClients(HttpClient httpClient, ICommonRepository commonRepository, SystemDbContext systemDb)
        {
            _httpClient = httpClient;
            _commonRepository = commonRepository;
            _systemDb = systemDb;
        }


        public async Task<SmsSendHtml_Result> SendSmsAsync(SmsSendHtml_Criteria criteria)
        {
            if (criteria == null) throw new ArgumentNullException(nameof(criteria));

            string requestJson = "";
            string raw = "";
            HttpResponseMessage? resp = null;
            string url = _httpClient.BaseAddress + "v2/";
            //string url_log = _httpClient.BaseAddress + url;
            try
            {
                var config = await _commonRepository.SystemConfigs(new Common_SystemConfigs_Criteria
                {
                    ConfigCodes = new List<string> { "SMS_UserName", "SMS_Password", "SMS_From", "SMS_Datacoding" }
                });

                if (string.IsNullOrWhiteSpace(criteria.To)) throw new ArgumentException("to is required");
                if (string.IsNullOrWhiteSpace(criteria.Text)) throw new ArgumentException("text is required");




                byte[] utf16BeBytes = Encoding.BigEndianUnicode.GetBytes(criteria.Text);

                StringBuilder urlEncodedMessage = new StringBuilder();
                foreach (byte b in utf16BeBytes)
                {

                    urlEncodedMessage.Append($"%{b:X2}");
                }

                string finalEncodedString = urlEncodedMessage.ToString();
                //Console.WriteLine($"Encoded string for API: {finalEncodedString}");

                var form = new Dictionary<string, string?>
                {
                    ["username"] = config.FirstOrDefault(d => d.ConfigCode == "SMS_UserName")?.ValueVarchar,
                    ["passwd"] = config.FirstOrDefault(d => d.ConfigCode == "SMS_Password")?.ValueVarchar,
                    ["from"] = config.FirstOrDefault(d => d.ConfigCode == "SMS_From")?.ValueVarchar,
                    ["to"] = criteria.To,
                    ["text"] = finalEncodedString,
                    ["datacoding"] = config.FirstOrDefault(d => d.ConfigCode == "SMS_Datacoding")?.ValueVarchar,
                    ["datetime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ["resultmode"] = "html"
                };

                // เก็บ Request เป็น JSON (จะเก็บ form อย่างเดียวก็ได้)

                using var content = new FormUrlEncodedContent(form!);
                var form_log = new Dictionary<string, string?>
                {
                    ["username"] = "",
                    ["passwd"] = "",
                    ["from"] = config.FirstOrDefault(d => d.ConfigCode == "SMS_From")?.ValueVarchar,
                    ["to"] = "",
                    ["text"] = criteria.Text,
                    ["datacoding"] = config.FirstOrDefault(d => d.ConfigCode == "SMS_Datacoding")?.ValueVarchar,
                    ["datetime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ["resultmode"] = "html"
                };
                requestJson = JsonSerializer.Serialize(form_log, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                resp = await _httpClient.PostAsync(url, content);

                raw = (await resp.Content.ReadAsStringAsync()) ?? "";
                var body = raw.Trim();

                var result = new SmsSendHtml_Result
                {
                    IsSuccessStatusCode = resp.IsSuccessStatusCode,
                    HttpStatusCode = resp.StatusCode,
                    RawContent = raw,
                    IsOk = resp.StatusCode == System.Net.HttpStatusCode.OK
                };



                // ถ้า HTTP ไม่ผ่าน
                if (!resp.IsSuccessStatusCode)
                    throw new HttpRequestException($"Failed to send SMS. HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}. Response: {raw}");

                // ถ้า HTTP 200 แต่ body ไม่ใช่ OK
                if (!result.IsOk)
                    throw new HttpRequestException($"SMS Gateway response is not OK. Response: {raw}");



                // ===== INSERT LOG (success / fail) =====
                var log = new TbApiSmsLog
                {
                    Id = Guid.NewGuid(),
                    Url = url,
                    ResponseStatus = $"{(int)resp.StatusCode} {resp.StatusCode}",
                    RequsestJson = requestJson,
                    ResponseJson = raw,
                    Error = (!resp.IsSuccessStatusCode || !result.IsOk)
                            ? $"Gateway not OK. Body: {body}"
                            : null,
                    CreateTime = DateTime.Now
                };

                _systemDb.Set<TbApiSmsLog>().Add(log);
                await _systemDb.SaveChangesAsync();


                return result;
            }
            catch (Exception ex)
            {
                // ===== INSERT LOG (exception) =====
                try
                {
                    var log = new TbApiSmsLog
                    {
                        Id = Guid.NewGuid(),
                        Url = url,
                        ResponseStatus = resp != null ? $"{(int)resp.StatusCode} {resp.StatusCode}" : "EXCEPTION",
                        RequsestJson = string.IsNullOrWhiteSpace(requestJson)
                                        ? JsonSerializer.Serialize(criteria)
                                        : requestJson,
                        ResponseJson = raw,
                        Error = ex.ToString(),
                        CreateTime = DateTime.Now
                    };

                    _systemDb.Set<TbApiSmsLog>().Add(log);
                    await _systemDb.SaveChangesAsync();
                }
                catch
                {

                }

                throw;
            }
        }
    }
}
