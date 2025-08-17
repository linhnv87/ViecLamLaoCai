namespace UploadFileServer.Models.Entity
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EIMEntities : DbContext
    {
        public EIMEntities()
            : base("name=EIMEntities")
        {
        }

        public virtual DbSet<ESOrganization> ESOrganizations { get; set; }
        public virtual DbSet<ConfigDatabas> ConfigDatabases { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }

    public partial class DynamicEntities : DbContext
    {
        public DynamicEntities(string connectionString)
            : base(connectionString)
        {
        }

        public virtual DbSet<DocumentOutAttachment> DocumentOutAttachments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
