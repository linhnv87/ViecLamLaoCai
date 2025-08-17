using Core;
using Core.Domains;
using Database.Models;
using Microsoft.Extensions.Options;
using Repositories;
using Services;
using System.Data.Entity;
using ViettelSMSServiceReference;

namespace QuanLyToTrinh.SMSService
{
    public interface ISMSService
    {
        Task<List<bool>> SendSMS(int docId, int type);
        Task<List<bool>> AutoSMSReminder();
        Task<bool> TestSMSSending();
        Task<List<bool>> SendSMSV2(int docId, int type);
    }
    public class SMSService:ISMSService
    {
        private HttpClient _client;
        private HttpRequestMessage _request;
        private ILogger<SMSService> _logger;
        private readonly SMSSettingModel _smsSettings;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserInRoleRepository _userInRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentReviewRepository _documentReviewRepository;
        private readonly ISMSLogRepository _smsLogRepository;
        private readonly QLTTrContext _context;
        private readonly IConfiguration _configuration;
        private readonly ITblDepartmentRepository _tblDepartmentRepository;
        public SMSService(ILogger<SMSService> logger, IOptions<SMSSettingModel> smsSettings, IRoleRepository roleRepo, IUserInRoleRepository userInRoleRepo, IUserRepository userRepo,
            IDocumentRepository docRepo, ISMSLogRepository smsLogRepository, IDocumentReviewRepository documentReviewRepository, QLTTrContext context, IConfiguration configuration, ITblDepartmentRepository tblDepartmentRepository)
        {
            _logger = logger;
            _smsSettings = smsSettings.Value;
            _roleRepository = roleRepo;
            _userRepository = userRepo;
            _userInRoleRepository = userInRoleRepo;
            _documentRepository = docRepo;
            _documentReviewRepository = documentReviewRepository;
            _smsLogRepository = smsLogRepository;
            _context = context;
            _configuration = configuration;
            _tblDepartmentRepository = tblDepartmentRepository;
        }
        public async Task<List<bool>> SendSMSV2(int docId, int type)
        {
            var result = new List<bool>();

            var usersHaveReviewed = (await _documentReviewRepository.GetMulti(x => x.DocId == docId && (x.SubmitCount.HasValue&&x.SubmitCount>0 || x.IsActiveSMS == true) && x.ReviewResult==null))
                        .Select(x => x.UserId.Value)
                        .Distinct()
                        .ToList();
            var sumbitCount = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);
            var allUsers = _userRepository.GetAll().Where(x=>x.IsLockedout != true).ToList();
            var allUssersInRoles = _userInRoleRepository.GetAll();
            var allRoles = _roleRepository.GetAll();
            var targetUsers = new List<AppUser>();
            targetUsers = allUsers.Where(x => usersHaveReviewed.Contains(x.UserId)).ToList();
            var message = "";
            var hostUrlConfig = _configuration["AppSettings:HostURL"];
            var hostUrl = $"{hostUrlConfig}/admin/to-trinh/xin-y-kien/document-detail/" + docId;
            //var hostUrl = "https://xinykien.yenbai.dcs.vn/admin/to-trinh/xin-y-kien/document-detail/" + docId;
            var document = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);
            if (document == null) throw new System.Exception("Không tìm thấy tờ trình");
            var endTime = document.DateEndApproval!.Value.ToString("dd/MM/yyyy HH:mm");
            switch (type)
            {
                case 0:
                    message = $"Thông báo hạn phê duyệt: Tờ trình {document.Title} có hạn phê duyệt đến hết ngày {endTime}, vui lòng truy cập {hostUrl} để xem chi tiết";
                    break;
                case 1:
                    message = $"Thông báo: Có tờ trình xin ý kiến mới được gửi đến đ/c, hạn phê duyệt đến hết ngày {endTime}. Chi tiết tại:{hostUrl}";
                    break;
                case 2:
                    message = $"Thông báo: Có tờ trình xin ý kiến sắp hết hạn phê duyệt. Chi tiết tại:{hostUrl}";
                    break;
            }
            //message = ChuyenCoDauThanhKhongDau(message);
            var userCreatedDoc = allUsers.FirstOrDefault(u => u.UserId == document.CreatedBy);
            if (userCreatedDoc != null && !string.IsNullOrEmpty(userCreatedDoc.PhoneNumber))
            {
                targetUsers.Add(userCreatedDoc);
            }
            var specialUsers = new Dictionary<string, string>
            {
                { "Trần Huy Tuấn", "Cao Ngọc Minh" },
                { "Tạ Văn Long", "Hoàng Hiệp" }
            };
            var sentPhoneNumbers = new HashSet<string>();
     
            var cvpRoles = new[] { AppRoleNames.CHANH_VAN_PHONG, AppRoleNames.PHO_CHANH_VAN_PHONG };
            var cvpRoleIds = allRoles.Where(r => cvpRoles.Contains(r.RoleName)).Select(r => r.RoleId).ToList();

            var cvpUserIds = allUssersInRoles.Where(ur => cvpRoleIds.Contains(ur.RoleId))
                                             .Select(ur => ur.UserId)
                                             .Distinct()
                                             .ToList();

            var additionalUsers = allUsers.Where(u => cvpUserIds.Contains(u.UserId)).ToList();

          
            var department = await _tblDepartmentRepository.FirstOrDefaultAsync(x => x.Id == userCreatedDoc.DepartmentId);

            if (department != null)
            {
              
                var tpPtpRoles = new[] { AppRoleNames.TRUONG_PHONG, AppRoleNames.PHO_TRUONG_PHONG };
                var tpPtpRoleIds = allRoles.Where(r => tpPtpRoles.Contains(r.RoleName)).Select(r => r.RoleId).ToList();

               
                var tpPtpUserIds = allUssersInRoles.Where(ur => tpPtpRoleIds.Contains(ur.RoleId))
                                                  .Select(ur => ur.UserId)
                                                  .Distinct()
                                                  .ToList();

                var tpPtpUsers = allUsers.Where(u => tpPtpUserIds.Contains(u.UserId) && u.DepartmentId == department.Id).ToList();
                additionalUsers.AddRange(tpPtpUsers);
            }

            foreach (var item in targetUsers)
            {
                if (specialUsers.ContainsKey(item.UserFullName))
                {
                    var targetUserFullName = specialUsers[item.UserFullName];
                    var targetUser = allUsers.FirstOrDefault(u => u.UserFullName == targetUserFullName);

                    if (targetUser != null && !sentPhoneNumbers.Contains(targetUser.PhoneNumber))
                    {
                        bool res = await SendSMSAndLogAsync(message, item.PhoneNumber, docId, type == 1 ? 0 : 1, (int)sumbitCount.SubmitCount);
                        bool res1 = await SendSMSAndLogAsync(message, targetUser.PhoneNumber, docId, type == 1 ? 0 : 1, (int)sumbitCount.SubmitCount);
                        result.Add(res1);
                        result.Add(res);
                        sentPhoneNumbers.Add(targetUser.PhoneNumber);
                        sentPhoneNumbers.Add(item.PhoneNumber);
                    }
                }        
                if (!sentPhoneNumbers.Contains(item.PhoneNumber))
                {
                    bool res2 = await SendSMSAndLogAsync(message, item.PhoneNumber, docId, type == 1 ? 0 : 1, (int)sumbitCount.SubmitCount);
                    result.Add(res2);
                    sentPhoneNumbers.Add(item.PhoneNumber);
                }
            }
            foreach (var roleUser in additionalUsers)
            {
                if (!sentPhoneNumbers.Contains(roleUser.PhoneNumber))
                {
                    bool res3 = await SendSMSAndLogAsync(message, roleUser.PhoneNumber, docId, type == 1 ? 0 : 1, (int)sumbitCount.SubmitCount);
                    result.Add(res3);
                    sentPhoneNumbers.Add(roleUser.PhoneNumber);
                }
            }
            return result;
        }
        async Task<bool> SendSMSAndLogAsync(string message, string phoneNumber, int docId, int type, int submitCount)
        {
            var res = await SendSMS_ViettelAsync(message, phoneNumber, docId, type, submitCount);
            if (res)
            {
                _logger.LogInformation($"Sending sms message to number: {phoneNumber} - Succeeded");
            }
            else
            {
                _logger.LogError($"Sending sms message to number: {phoneNumber} - Failed");
            }
            return res;
        }
        public async Task<List<bool>> SendSMS(int docId, int type)
        {
            var result = new List<bool>();
            var targetUsers = new List<AppUser>();
            var message = "";
            //var hostUrl = "https://totrinh.yenbai.net.vn/admin/to-trinh/3/document-detail/" + docId;
            var hostUrl = "https://bcsd.yenbai.gov.vn/admin/to-trinh/3/document-detail/" + docId;
            var roleNames = new List<string>() { "Approver", "General Approver", "Admin", "General Specialist", "Specialist"};
            var document = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);

            if (document == null) throw new System.Exception("Không tìm thấy tờ trình");
            var endTime = document.DateEndApproval!.Value.Day + "/" + document.DateEndApproval!.Value.Month + "/" + document.DateEndApproval!.Value.Year;
            if (type == 1) //Specialist send document to approve => Send sms to Approver, General Approver, General Specialist
            {
                roleNames = new List<string>() { "Approver", "General Approver", "General Specialist", "Admin", "General" };
                var targetRoleIds = (await _roleRepository.GetMulti(x => roleNames.Contains(x.RoleName))).Select(x => x.RoleId);
                var targetUserIdList = (await _userInRoleRepository.GetMulti(x => targetRoleIds.Contains(x.RoleId))).Select(x => x.UserId);
                targetUsers = await _userRepository.GetMulti(x => targetUserIdList.Contains(x.UserId));
                message = $"Thông báo tờ trình mới: Tờ trình {document.Title} đã được trình xin ý kiến, hạn phê duyệt đến hết ngày {endTime}, vui lòng truy cập {hostUrl} để xem chi tiết";                
            }
            else if(type == 2 || type == 0) //General specialist reminds Approvers to review document
            {
                roleNames = new List<string>() { "Approver", "General Approver", "Admin" };
                var targetRoleIds = (await _roleRepository.GetMulti(x => roleNames.Contains(x.RoleName))).Select(x => x.RoleId);
                var targetUserIdList = (await _userInRoleRepository.GetMulti(x => targetRoleIds.Contains(x.RoleId))).Select(x => x.UserId);

                var usersHaveDone = (await _documentReviewRepository.GetMulti(x => x.DocId == docId)).Select(x => x.UserId).Distinct();
                targetUserIdList = targetUserIdList.Where(x => !usersHaveDone.Contains(x));
                targetUsers = await _userRepository.GetMulti(x => targetUserIdList.Contains(x.UserId));

                if(type == 0)
                {
                    message = $"Thông báo hạn phê duyệt: Tờ trình {document.Title} có hạn phê duyệt đến hết ngày {endTime}, vui lòng truy cập {hostUrl} để xem chi tiết";                
                }
                if(type == 2)
                {
                    message = $"Nhắc việc: Tờ trình {document.Title} có hạn phê duyệt đến hết ngày {endTime}, vui lòng truy cập {hostUrl} để xem chi tiết";
                }
            }            
            //else if(type == 3) //Document's status updated to Approved
            //{
            //    //roleNames = new List<string>() { "Admin", "General Approver" };
            //    roleNames = new List<string>() { "Approver", "General Approver", "Admin", "General Specialist", "General" };
            //    var targetRoleIds = (await _roleRepository.GetMulti(x => roleNames.Contains(x.RoleName))).Select(x => x.RoleId);
            //    var targetUserIdList = (await _userInRoleRepository.GetMulti(x => targetRoleIds.Contains(x.RoleId))).Select(x => x.UserId);
            //    targetUsers = await _userRepository.GetMulti(x => targetUserIdList.Contains(x.UserId) || x.UserId == document.CreatedBy);
            //    message = $"Kết quả - Duyệt - Tờ trình {document.Title} đã được duyệt và đang chờ trưởng ban thông qua, truy cập {hostUrl} để xem thông tin chi tiết";                
            //}
            //else if (type == 4) //Document's status updated to Declined
            //{
            //    roleNames = new List<string>() { "Admin", "General Specialist", "General" };
            //    var targetRoleIds = (await _roleRepository.GetMulti(x => roleNames.Contains(x.RoleName))).Select(x => x.RoleId);
            //    var targetUserIdList = (await _userInRoleRepository.GetMulti(x => targetRoleIds.Contains(x.RoleId))).Select(x => x.UserId);
            //    targetUsers = await _userRepository.GetMulti(x => targetUserIdList.Contains(x.UserId) || x.UserId == document.CreatedBy);
            //    message = $"Tờ trình {document.Title} không được duyệt, vui lòng truy cập {hostUrl} để xem thông tin chi tiết";
            //}
            //else if (type == 5) //Document's status updated to Overdued
            //{
            //    roleNames = new List<string>() { "Admin", "General Specialist", "General" };
            //    var targetRoleIds = (await _roleRepository.GetMulti(x => roleNames.Contains(x.RoleName))).Select(x => x.RoleId);
            //    var targetUserIdList = (await _userInRoleRepository.GetMulti(x => targetRoleIds.Contains(x.RoleId))).Select(x => x.UserId);
            //    targetUsers = await _userRepository.GetMulti(x => targetUserIdList.Contains(x.UserId) || x.UserId == document.CreatedBy);
            //    message = $"Tờ trình {document.Title} sắp quá hạn duyệt, truy cập {hostUrl} để xem chi tiết";
            //}
            //else if (type == 6)
            //{
            //    roleNames = new List<string>() { "Approver", "General Approver", "Admin", "General Specialist", "General" };
            //    var targetRoleIds = (await _roleRepository.GetMulti(x => roleNames.Contains(x.RoleName))).Select(x => x.RoleId);
            //    var targetUserIdList = (await _userInRoleRepository.GetMulti(x => targetRoleIds.Contains(x.RoleId))).Select(x => x.UserId);
            //    targetUsers = await _userRepository.GetMulti(x => targetUserIdList.Contains(x.UserId) || x.UserId == document.CreatedBy);
            //    //targetUsers = await _userRepository.GetMulti(x => x.UserId.ToString().ToLower() == document.CreatedBy.ToString().ToLower());
            //    //message = $"Kết quả - Thông qua - Tờ trình {document.Title} đã được phê duyệt thông qua và chờ ban hành nghị quyết, truy cập {hostUrl} để xem thông tin chi tiết";
            //    message = $"Tờ trình {document.Title} đã được ban hành, vui lòng truy cập {hostUrl} để xem thông tin chi tiết";
            //}
            //else if (type == 7)
            //{
            //    roleNames = new List<string>() { "Approver", "General Approver", "Admin", "General Specialist", "General" };
            //    var targetRoleIds = (await _roleRepository.GetMulti(x => roleNames.Contains(x.RoleName))).Select(x => x.RoleId);
            //    var targetUserIdList = (await _userInRoleRepository.GetMulti(x => targetRoleIds.Contains(x.RoleId))).Select(x => x.UserId);
            //    targetUsers = await _userRepository.GetMulti(x => targetUserIdList.Contains(x.UserId) || x.UserId == document.CreatedBy);
            //    //targetUsers = await _userRepository.GetMulti(x => x.UserId.ToString().ToLower() == document.CreatedBy.ToString().ToLower());
            //    message = $"Kết quả - Đã ban hành nghị quyết - Tờ trình {document.Title} đã được ban hành nghị quyết, truy cập {hostUrl} để xem thông tin chi tiết";
            //}
            //else if (type == 8)
            //{
            //    roleNames = new List<string>() { "Admin", "General Specialist", "General", "General Approver", "Approver" };
            //    var targetRoleIds = (await _roleRepository.GetMulti(x => roleNames.Contains(x.RoleName))).Select(x => x.RoleId);
            //    var targetUserIdList = (await _userInRoleRepository.GetMulti(x => targetRoleIds.Contains(x.RoleId))).Select(x => x.UserId);
            //    targetUsers = await _userRepository.GetMulti(x => targetUserIdList.Contains(x.UserId) || x.UserId == document.CreatedBy);
            //    //targetUsers = await _userRepository.GetMulti(x => x.UserId.ToString().ToLower() == document.CreatedBy.ToString().ToLower());
            //    message = $"Kết quả - Tờ trình bị trả lại - Tờ trình {document.Title} đã bị trả lại cho chuyên viên, truy cập {hostUrl} để xem thông tin chi tiết";
            //}
            //else if (type == 9)
            //{
            //    roleNames = new List<string>() { "Admin", "General Specialist", "General", "General Approver", "Approver" };
            //    var targetRoleIds = (await _roleRepository.GetMulti(x => roleNames.Contains(x.RoleName))).Select(x => x.RoleId);
            //    var targetUserIdList = (await _userInRoleRepository.GetMulti(x => targetRoleIds.Contains(x.RoleId))).Select(x => x.UserId);
            //    targetUsers = await _userRepository.GetMulti(x => targetUserIdList.Contains(x.UserId) || x.UserId == document.CreatedBy);
            //    //targetUsers = await _userRepository.GetMulti(x => x.UserId.ToString().ToLower() == document.CreatedBy.ToString().ToLower());
            //    message = $"Thông báo - Tờ trình {document.Title} đã được Ban Cán sự Đảng đồng ý cho Thu hồi, vui lòng truy cập {hostUrl} để xem chi tiết";
            //}
            message = ChuyenCoDauThanhKhongDau(message);
            foreach (var item in targetUsers)
            {                
                var res = await SendSMS_ViettelAsync(message, item.PhoneNumber,docId,1,1);                
                if (res)
                {
                    _logger.LogInformation($"Sending sms message to number: {item.PhoneNumber} - Succeeded");
                }
                else
                {
                    _logger.LogError($"Sending sms message to number: {item.PhoneNumber} - Failed");
                }
                result.Add(res);
            }
            return result;
        }
        public async Task<List<bool>> AutoSMSReminder()
        {
            var result = new List<bool>();
            var currentDate = new DateTime();
            //var docList = (await _documentRepository.GetMulti(x => (x.StatusCode == 3 || x.StatusCode == 7) && ((DateTime)x.DateEndApproval).AddDays(2) < currentDate)).Select(x => x.Id).ToList();
            //if (docList != null && docList.Count > 0)
            //{
            //    foreach(var doc in docList)
            //    {
            //        var z = await SendSMS(doc, 2);
            //        result.AddRange(z);
            //    }
            //}
            return result;
        }
        public async Task<bool> TestSMSSending()
        {
            var res = await SendSMS_ViettelAsync("TEST OK", "0363107478",1,1,1);
            return res;
        }
        private async Task<bool> SendSMS_ViettelAsync(string message, string toPhonenumber,int docId,int type,int sumbitCount)
        {
            bool rs = true;
            string requestId = "1";
            string cmdCode = "bulksms";
            string errMessage = "";
            try
            {

                var test = new CcApiClient();
                var outcome =await test.wsCpMtAsync("smsbrand_tinhuyyenbai", "123456aB@", "YBI_VANPHONGTINHUY", requestId, toPhonenumber, toPhonenumber, "TINHUY-YB", cmdCode, message, "1");
                //var outcome = await test.wsCpMtAsync(_smsSettings.Username, _smsSettings.Password, _smsSettings.CPCode, requestId, toPhonenumber, toPhonenumber, _smsSettings.BrandName, cmdCode, message, _smsSettings.ContentType);
                //_logger.LogError($"Username {_smsSettings.Username},Password {_smsSettings.Password} ,CPCode {_smsSettings.CPCode} ,BrandName {_smsSettings.BrandName}, {_smsSettings.ToString()}");
                _logger.LogInformation($"Username smsbrand_sottttyenbai,Password 123456a@ ,CPCode YBI_VANPHONGTINHUY ,BrandName SOTTTT_YB");
                if (outcome.@return.message.Contains("Insert MT_QUEUE: OK") == false)
                {
                    var messageTV = Constants.Viettel_ErrorList.Where(x => x.Key.Contains(outcome.@return.message) == true).FirstOrDefault();
                    if (messageTV != null)
                    {
                        _logger.LogError($"{messageTV.Value}");
                        errMessage = messageTV.Value;
                    }
                    else
                    {
                        _logger.LogError($"wsCpMtAsync outcome Viettel {outcome.@return.message}");
                        errMessage = outcome.@return.message;
                    }
                    rs = false;
                }
                _logger.LogInformation($"Info wsCpMtAsync outcome Viettel {outcome.@return.message}");
                errMessage = outcome.@return.message;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Send sms to {toPhonenumber} get error: {ex}");
                errMessage = ex.Message;
                rs = false;
            }
            var data = new TblSMSLog()
            {
                Id = 0,
                ErrorMessage = errMessage,
                DocId = docId,
                Type = type,
                IsSucceeded = rs,
                SubmitCount = sumbitCount,
                PhoneNumber = toPhonenumber,
                Created = DateTime.Now
            };
            await _smsLogRepository.AddAsync(data);
            await _smsLogRepository.SaveChanges();
            return rs;
        }

        private static readonly string[] VietNamChar = new string[]
       {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
       };

        public static string FilterChar(string str)
        {
            str = str.Trim();
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                {
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
                }
            }
            str = str.Replace("(", "");
            str = str.Replace(")", "");
            str = str.Replace("“", "");
            str = str.Replace("”", "");
            str = str.Replace(" ", "-");
            str = str.Replace("--", "-");
            str = str.Replace("?", "");
            str = str.Replace("&", "");
            str = str.Replace(",", "");
            str = str.Replace(":", "");
            str = str.Replace("!", "");
            str = str.Replace("'", "");
            str = str.Replace("\"", "");
            str = str.Replace("%", "");
            str = str.Replace("#", "");
            str = str.Replace("$", "");
            str = str.Replace("*", "");
            str = str.Replace("`", "");
            str = str.Replace("~", "");
            str = str.Replace("@", "");
            str = str.Replace("^", "");
            str = str.Replace("/", "");
            str = str.Replace(">", "");
            str = str.Replace("<", "");
            str = str.Replace("[", "");
            str = str.Replace("]", "");
            str = str.Replace(";", "");
            str = str.Replace("+", "");
            return str;//.ToLower();
        }

        public static string ChuyenCoDauThanhKhongDau(string str)
        {
            str = str.Trim();
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                {
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
                }
            }
            //str = str.Replace(" ", "-");
            str = str.Replace("--", "-");
            //str = str.Replace("?", "");
            str = str.Replace("&", "");
            //str = str.Replace(",", "");
            //str = str.Replace(":", "");
            //str = str.Replace("!", "");
            str = str.Replace("'", "");
            str = str.Replace("\"", "");
            str = str.Replace("%", "");
            //str = str.Replace("#", "");
            str = str.Replace("$", "");
            str = str.Replace("*", "");
            str = str.Replace("`", "");
            str = str.Replace("~", "");
            str = str.Replace("@", "");
            str = str.Replace("^", "");
            //str = str.Replace(".", "");
            //str = str.Replace("/", "");
            str = str.Replace(">", "");
            str = str.Replace("<", "");
            str = str.Replace("[", "");
            str = str.Replace("]", "");
            str = str.Replace(";", "");
            str = str.Replace("+", "");
            return str;//.ToLower();
        }
    }
}
