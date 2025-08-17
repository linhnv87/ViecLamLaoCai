using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class FieldDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; } = DateTime.Now;
        public bool? Deleted { get; set; }
    }
    public class UpdateOrderFieldsDTO
    {
        public int Id { get; set; }
        public int Order { get; set; }
    }

}
