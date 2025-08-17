using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class TblDocumentTypesDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
    }
    public class UpdateOrderDocumentTypesDTO
    {
        public long Id { get; set; }
        public int Order { get; set; }
    }

}
