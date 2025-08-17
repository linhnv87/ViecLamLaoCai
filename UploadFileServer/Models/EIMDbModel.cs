namespace UploadFileServer.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EIMDbModel : DbContext
    {
        public EIMDbModel()
            : base("name=EIMModel")
        {
        }

        public virtual DbSet<ESOrganization> ESOrganizations { get; set; }
        public virtual DbSet<ConfigDatabas> ConfigDatabases { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ESOrganization>()
                .Property(e => e.OrgID)
                .IsUnicode(false);

            modelBuilder.Entity<ESOrganization>()
                .HasMany(e => e.ConfigDatabases)
                .WithOptional(e => e.ESOrganization)
                .HasForeignKey(e => e.OrganizationID);

            modelBuilder.Entity<ESOrganization>()
                .HasMany(e => e.ESOrganizations1)
                .WithOptional(e => e.ESOrganization1)
                .HasForeignKey(e => e.ParentId);
        }
    }
}
