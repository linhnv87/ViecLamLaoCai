using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domains
{
    public class EmailSettingModel
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class SMSSettingModel
    {
        public string Username { get; set; }

        public string Phonenumber { get; set; }

        public string ContentType { get; set; }

        public string Password { get; set; }

        public string BrandName { get; set; }
        public string Provider { get; set; }
        public string CPCode { get; set; }
        public string LinkURL { get; set; }
    }

    public class SmsResponseDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string MessageId { get; set; }
    }
}
