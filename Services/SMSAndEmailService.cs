using Core.Domains;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Services
{
    public interface ISMSAndEmailService
    {

    }
    public class SMSAndEmailService : ISMSAndEmailService
    {
        private HttpClient _client;
        private HttpRequestMessage _request;
        private ILogger<SMSAndEmailService> _logger;
        private readonly SMSSettingModel _smsSettings;
        public SMSAndEmailService(ILogger<SMSAndEmailService> logger, IOptions<SMSSettingModel> smsSettings)
        {   
            _logger = logger;            
        }        
        private async Task<bool> SendSMS_VNPTAsync(string linkURL, string message, string toPhonenumber)
        {

            bool rs;
            try
            {
                _client = new HttpClient();
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(linkURL),
                };
                var content = new
                {
                    username = _smsSettings.Username,
                    password = _smsSettings.Password,
                    type = _smsSettings.ContentType,
                    brandname = _smsSettings.BrandName,
                    message = message,
                    phonenumber = toPhonenumber
                };
                var json = JsonConvert.SerializeObject(content);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                _request.Content = stringContent;
                var response = await _client.SendAsync(_request);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var responseObj = JsonConvert.DeserializeObject<SmsResponseDto>(result);

                _logger.LogInformation($"responseObj.MessageId = {responseObj.MessageId}");
                _logger.LogInformation($"responseObj.Id = {responseObj.Id}");
                _logger.LogInformation($"responseObj.Description = {responseObj.Description}");

                if (responseObj.Id == 1)
                {
                    _logger.LogInformation("Gửi Sms thành công");
                    rs = true;
                }
                else if (responseObj.Id == 2)
                {
                    _logger.LogError("Số điện thoại sai format");
                    rs = false;
                }
                else if (responseObj.Id == 4)
                {
                    _logger.LogError("Brandname chưa active");
                    rs = false;
                }
                else if (responseObj.Id == 15)
                {
                    _logger.LogError("Kết nối Gateway lỗi");
                    rs = false;
                }
                else if (responseObj.Id == 17)
                {
                    _logger.LogError("Template không hợp lệ");
                    rs = false;
                }
                else if (responseObj.Id == 25)
                {
                    _logger.LogError("sai mạng, hoặc lable không hợp lệ");
                    rs = false;
                }
                else if (responseObj.Id == 100)
                {
                    _logger.LogError("database error");
                    rs = false;
                }
                else
                {
                    rs = false;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Send sms to {toPhonenumber} get error: {ex}");
                rs = false;
            }
            return rs;
        }
    }
}
