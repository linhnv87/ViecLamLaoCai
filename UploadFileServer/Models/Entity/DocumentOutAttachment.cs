namespace UploadFileServer.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Document.DocumentOutAttachments")]
    public partial class DocumentOutAttachment
    {
        public int DocumentOutAttachmentID { get; set; }

        public Guid? DocumentOutAttachmentGuidID { get; set; }

        [StringLength(50)]
        public string DocumentOutGuidID { get; set; }

        public int? DocumentOutID { get; set; }

        [StringLength(250)]
        public string FileName { get; set; }

        public byte[] Attachment { get; set; }

        [StringLength(50)]
        public string FileType { get; set; }

        [StringLength(10)]
        public string FileExtension { get; set; }

        public byte[] FileConverted { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(250)]
        public string CreatedBy { get; set; }

        [StringLength(250)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool? IsLocked { get; set; }

        public bool? CurrentVersion { get; set; }

        public int? Version { get; set; }

        public int? IsSubmit { get; set; }

        [StringLength(50)]
        public string EmployeeID { get; set; }

        public int? ParentID { get; set; }
    }
}
