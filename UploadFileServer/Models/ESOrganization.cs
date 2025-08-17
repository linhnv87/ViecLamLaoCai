namespace UploadFileServer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("eim.ESOrganizations")]
    public partial class ESOrganization
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ESOrganization()
        {
            ConfigDatabases = new HashSet<ConfigDatabas>();
            ESOrganizations1 = new HashSet<ESOrganization>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string OrgID { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int? ParentId { get; set; }

        [StringLength(50)]
        public string ParentCode { get; set; }

        public int? Ord { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        public bool? IsEmail { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        public bool? IsLocal { get; set; }

        [StringLength(250)]
        public string Key { get; set; }

        [StringLength(250)]
        public string AccessKey { get; set; }

        [StringLength(50)]
        public string LinkWeb { get; set; }

        public int? IsCategory { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfigDatabas> ConfigDatabases { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ESOrganization> ESOrganizations1 { get; set; }

        public virtual ESOrganization ESOrganization1 { get; set; }
    }
}
