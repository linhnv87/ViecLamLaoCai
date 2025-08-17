using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class TblComment
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public int? DocId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public bool? Deleted { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UserId { get; set; }        
    }
}
