using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models;

namespace Services.DTO
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public int? DocId { get; set; }
        public string UserName { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public bool? Deleted { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UserId { get; set; }
    }
}
