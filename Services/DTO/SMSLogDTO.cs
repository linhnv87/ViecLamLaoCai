using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class SMSLogDTO
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string ReceiverName { get; set; }
        public string MessageType { get; set; }
        public string Status { get; set; }
        public DateTime SentTime { get; set; }
        public string RoleName { get; set; }
        public string? ErrorMessage { get; set; }
    }
    public class SMSLogGroupDTO
    {
        public int? SubmitCount { get; set; }
        public List<SMSLogDTO> SMSLogs { get; set; }
    }
}
