using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class KeyPairSS
    {
        public KeyPairSS()
        { }
        public KeyPairSS(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public static class Constants
    {
        public static List<KeyPairSS> Viettel_ErrorList = new List<KeyPairSS>() {
          new KeyPairSS("Authenticate: Cp_code: NULL_OR_BLANK","Thiếu thông tin cp_code"),
          new KeyPairSS("Authenticate: UserName: NULL_OR_BLANK","Thiếu thông tin user_name"),
          new KeyPairSS("Authenticate: IP_INVALID (YOUR IP: XXXX)","IP XXXX của hệ thống bạn đang gửi tin chưa được đăng ký whitelist."),
          new KeyPairSS("Check RequestID: NULL_OR_BLANK","Thiếu thông tin RequestID"),
          new KeyPairSS("Authenticate: CP_CODE_NOT_FOUND","Thông tin cp_code không chính xác"),
          new KeyPairSS("Authenticate: WRONG_INFORMATION_AUTHENTICATE","Thông tin user/pass không chính xác"),
          new KeyPairSS("Check RequestID: REQUEST_ID_NOT_NUMBER","RequestID không đúng"),
          new KeyPairSS("Check UserID: NULL_OR_BLANK","Thiếu thông tin UserID"),
          new KeyPairSS("Check ReceiverID: NULL_OR_BLANK","Thiếu thông tin ReceiverID"),
          new KeyPairSS("Check ReceiverID: FORMAT_ERROR","ReceiverID không đúng"),
          new KeyPairSS("UserID_NOT_EQUAL_ReceiverID","UserID và ReceiverID phải giống nhau"),
          new KeyPairSS("Unable to check telco from input receiver","Không xác định được nhà mạng của thuê bao (do ReceiverID sai)"),
          new KeyPairSS("Length of ReceiverID is invalid.","ReceiveID không đúng (sai độ dài)"),
          new KeyPairSS("Check ServiceID: DUPLICATE MESSAGE","Tin nhắn bị lặp"),
          new KeyPairSS("Check ServiceID: ALIAS_INVALID:TELCO=XX","Sai thương hiệu hoặc thương hiệu chưa được khai báo cho nhà mạng tương ứng với thuê bao"),
          new KeyPairSS("Check CommandCode: NULL_OR_BLANK","Thiếu thông tin command_code"),
          new KeyPairSS("Check CommandCode: COMMAND_CODE_ERROR","Sai command_code"),
          new KeyPairSS("Check Content: NULL_OR_BLANK","Không có nội dung tin nhắn"),
          new KeyPairSS("Check Content: MAXLENGTH_LIMIT_XXXX_BYTE ","Độ dài tin vượt quá giới hạn (XXXX: số byte tối đa, YY là số byte nội dung tin mà bạn nhập)"),
          new KeyPairSS("Check Content: MSG_ERROR_CONTAIN_BLACKLIST","Nội dung chứa từ ngữ bị chặn"),
          new KeyPairSS("Check information error","Lỗi chung hệ thống"),
          new KeyPairSS("Check template: CONTENT_NOT_MATCH_TEMPLATE","Lỗi sai định dạng mẫu tin nhắn"),
        };        
    }

    public static class AppRoleNames
    {
        public const string ADMIN = "Admin";
        public const string SPECIALIST = "Specialist";
        public const string GENERAL_SPECIALIST = "General Specialist";
        public const string APPROVER = "Approver";
        public const string GENERAL_APPROVER = "General Approver";
        public const string OFFICER = "Officer";
        public const string BAN_CHAP_HANH = "ban-chap-hanh";
        public const string TRUONG_PHONG = "truong-phong";
        public const string PHO_TRUONG_PHONG = "pho-truong-phong";
        public const string PHO_CHANH_VAN_PHONG = "pho-chanh-van-phong";
        public const string CHANH_VAN_PHONG = "chanh-van-phong";
        public const string PHO_BI_THU = "pho-bi-thu";
        public const string BI_THU = "bi-thu";
        public const string CHUYEN_VIEN = "chuyen-vien";
        public const string VAN_THU = "van-thu";
        public const string BAN_THUONG_VU = "ban-thuong-vu";
    }

    public static class AppDocumentStatuses
    {
        public const string DU_THAO = "du-thao";
        public const string XIN_Y_KIEN = "xin-y-kien";
        public const string XIN_Y_KIEN_LAI = "xin-y-kien-lai";
        public const string PHE_DUYET = "phe-duyet";
        public const string KY_SO = "ky-so";
        public const string CHO_BAN_HANH = "cho-ban-hanh";
        public const string BAN_HANH = "ban-hanh";
        public const string KHONG_BAN_HANH = "khong-ban-hanh";
        public const string TRA_LAI = "tra-lai";
        public static List<string> GetAllStatuses()
        {
            return typeof(AppDocumentStatuses)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.IsLiteral && !f.IsInitOnly)
                .Select(f => f.GetValue(null)?.ToString())
                .ToList();
        }
    }

    public static class AppDocumentStatusOrder
    {
        private static readonly Dictionary<string, int> StatusOrderMap = new Dictionary<string, int>
        {
            { AppDocumentStatuses.PHE_DUYET, 3 },
            { AppDocumentStatuses.KY_SO, 4 },
            { AppDocumentStatuses.CHO_BAN_HANH, 5 },
            { AppDocumentStatuses.BAN_HANH, 6 }
        };
        private static readonly Dictionary<string, int> DraftOrderMap = new Dictionary<string, int>
        {
            { AppDocumentStatuses.DU_THAO, 1 },
            { AppDocumentStatuses.XIN_Y_KIEN, 2 },
        };
        private static readonly Dictionary<string, int> OpinionRequestOrderMap = new Dictionary<string, int>
{
    { AppDocumentStatuses.XIN_Y_KIEN_LAI, 9 },
    { AppDocumentStatuses.XIN_Y_KIEN, 2 },
};

        public static (int currentOrder, int? previousOrder, string previousOrderKey) GetPreviousOrder(string statusCode, string preStatusCode)
        {
            
            if (StatusOrderMap.TryGetValue(statusCode, out int currentOrder))
            {
                var previousOrderEntry = StatusOrderMap
                    .Where(entry => entry.Value < currentOrder)
                    .OrderByDescending(entry => entry.Value)
                    .FirstOrDefault();

                if (previousOrderEntry.Equals(default(KeyValuePair<string, int>)))
                {
                    previousOrderEntry = StatusOrderMap.First();
                }

                int? previousOrder = previousOrderEntry.Value;
                string previousOrderKey = previousOrderEntry.Key;

                return (currentOrder, previousOrder, previousOrderKey);
            }

            if (DraftOrderMap.TryGetValue(statusCode, out int currentDraftOrder))
            {
                if (preStatusCode == AppDocumentStatuses.DU_THAO || preStatusCode == AppDocumentStatuses.XIN_Y_KIEN || preStatusCode ==null)
                {
                    var previousOrderEntry = DraftOrderMap
                        .Where(entry => entry.Value < currentDraftOrder)
                        .OrderByDescending(entry => entry.Value)
                        .FirstOrDefault();

                    if (previousOrderEntry.Equals(default(KeyValuePair<string, int>)))
                    {
                        previousOrderEntry = StatusOrderMap.First();
                    }

                    int? previousOrder = previousOrderEntry.Value;
                    string previousOrderKey = previousOrderEntry.Key;

                    return (currentDraftOrder, previousOrder, previousOrderKey);
                }
            }

            if (OpinionRequestOrderMap.TryGetValue(statusCode, out int currentOpinionOrder))
            {
                
                if (preStatusCode == AppDocumentStatuses.XIN_Y_KIEN_LAI)
                {
                    int? previousOrder = 9;
                    string previousOrderKey = AppDocumentStatuses.XIN_Y_KIEN_LAI;

                    return (currentOpinionOrder, previousOrder, previousOrderKey);
                }
            }
            throw new ArgumentException($"Invalid status code: {statusCode}");
        }
    }

    public static class AppDocument
    {
        public const string EXCEL = "EXCEL";
        public const string PDF = "PDF";
        public static string FormatName = "yyyyMMdd_HHmmss";

        public static string EXCELFileNameExtention = ".xlsx";
        public static string EXCELOpenxmlformats = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public static string PDFFileNameExtention = ".pdf";
        public static string PDFOpenxmlformats = "application/pdf";
    }
}
