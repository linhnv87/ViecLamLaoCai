using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper
{
    public class DocumentsWithFieldInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FieldName { get; set; }
        public string UserName { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? DateEndApproval { get; set; }
    }
}
