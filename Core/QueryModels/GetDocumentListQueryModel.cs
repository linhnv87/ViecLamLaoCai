using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.QueryModels
{
    public class GetDocumentListQueryModel
    {
        public string? Status { get; set; }
        public string? CurrentUserId { get; set; }
        public bool IsCounting { get; set; }
    }
}
