using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.QueryModels
{
    public class SMSLogQueryModel
    {
        public int DocId { get; set; }
        public int Type { get; set; }
        public bool? IsSucceeded { get; set; }
    }
}
