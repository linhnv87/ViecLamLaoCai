using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class ToTrinhUpdateRequestDTO
    {
        public required int DocumentId { get; set; }
        public required List<Guid> Users { get; set; }
        public required string FromStatusCode { get; set; }
        public required string ToStatusCode { get; set; }
        public required Guid UserId { get; set; }
        public string? Comment { get; set; }

        // attach files
        public IFormFile[]? AttachmentFiles { get; set; }
    }
}
