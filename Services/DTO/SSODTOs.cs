using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class SSODTOs
    {
        public class SsoPostData
        {
            public string code { get; set; }
            public string redirect_uri { get; set; }
            public string access_token { get; set; }
            public string refresh_token { get; set; }
        }
        public class SsoErrorResponse
        {
            public string error { get; set; }
            public string error_description { get; set; }
        }
        public class TokenResponseData
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
            public string id_token { get; set; }
            public string token_type { get; set; }
            public int? expires_in { get; set; }
        }
        public class UserInfoResponseData
        {
            public string sub { get; set; }
            public string name { get; set; }
            public string preferred_username { get; set; }
            public string email { get; set; }
        }

        public class SsoGetSchedule
        {
            public string accessToken { get; set; }
            public string scheduleDate { get; set; }
        }

        public class SsoLoginToken
        {
            public string accessToken { get; set; }
        }
    }
}
