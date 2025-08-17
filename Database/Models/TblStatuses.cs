using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class TblStatuses
    {
        public int Id { get; set; }
        public string? StatusCode { get; set; }
        public string? Title { get; set; }
        public DateTime? Modified { get; set; }
        public bool? Deleted { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? Created { get; set; }
        public int Order { get; set; }
    }
}
