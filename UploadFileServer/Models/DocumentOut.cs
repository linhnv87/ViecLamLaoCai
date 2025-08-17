namespace UploadFileServer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Document.DocumentOuts")]
    public partial class DocumentOut
    {
        [Key]
        [Column(Order = 0)]
        public int DocumentOutID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid DocumentOutGuidID { get; set; }

        [StringLength(50)]
        public string DocumentOutNumber { get; set; }

        [StringLength(50)]
        public string DocumentOutBookID { get; set; }

        [StringLength(255)]
        public string DocumentOutBookName { get; set; }

        [StringLength(50)]
        public string DocumentOriginGuiID { get; set; }

        [StringLength(100)]
        public string NotationNumber { get; set; }

        [StringLength(50)]
        public string DepartmentID { get; set; }

        [StringLength(50)]
        public string DepartmentName { get; set; }

        [StringLength(50)]
        public string SendDepartmentName { get; set; }

        [StringLength(50)]
        public string SendDepartmentID { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        public int? PageAmount { get; set; }

        public int? Version { get; set; }

        public int? SendMethod { get; set; }

        [StringLength(500)]
        public string DocumentRelation { get; set; }

        [StringLength(255)]
        public string WorkspaceStore { get; set; }

        public int? PublishAmount { get; set; }

        public DateTime? DocumentDate { get; set; }

        public DateTime? ReceivedDate { get; set; }

        [StringLength(50)]
        public string DocumentTypeID { get; set; }

        [StringLength(50)]
        public string DocumentTypeName { get; set; }

        [StringLength(50)]
        public string UrgencyID { get; set; }

        public int? JobProfileID { get; set; }

        public int? SecurityLevel { get; set; }

        [StringLength(50)]
        public string ComposeID { get; set; }

        [StringLength(255)]
        public string ComposeName { get; set; }

        [StringLength(50)]
        public string ApproverBy { get; set; }

        public int? SignByID { get; set; }

        [StringLength(50)]
        public string SignByName { get; set; }

        [StringLength(50)]
        public string JobTitle { get; set; }

        [StringLength(50)]
        public string ProcessorBy { get; set; }

        [StringLength(50)]
        public string FollowedBy { get; set; }

        [StringLength(50)]
        public string StatusOutID { get; set; }

        [StringLength(50)]
        public string StatusOutName { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool? IsLocked { get; set; }

        [StringLength(50)]
        public string WorkFlowID { get; set; }

        public bool? IsStored { get; set; }

        public bool? IsPrint { get; set; }

        public bool? IsDigitalSign { get; set; }

        public bool? IsMakeNumber { get; set; }

        public bool? IsPublish { get; set; }

        public bool? IsDraft { get; set; }

        public bool? Status { get; set; }

        public string QRCode { get; set; }

        [StringLength(50)]
        public string Barcode { get; set; }

        public bool? Submission { get; set; }

        public int? Location { get; set; }

        public bool? IsComment { get; set; }

        [StringLength(50)]
        public string CommentPerson { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool? CheckSign { get; set; }
    }
}
