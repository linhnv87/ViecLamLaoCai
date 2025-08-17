namespace UploadFileServer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("esb.ConfigDatabases")]
    public partial class ConfigDatabas
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Database { get; set; }

        public int? OrganizationID { get; set; }

        [Column(TypeName = "ntext")]
        public string ConnectString { get; set; }

        [StringLength(50)]
        public string OrganizationCode { get; set; }

        [ForeignKey("OrganizationID")]
        public virtual ESOrganization ESOrganization { get; set; }
    }
}
