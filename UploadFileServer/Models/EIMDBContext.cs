using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace UploadFileServer.Models
{
    public partial class EIMDBContext : DbContext
    {
        public EIMDBContext()
           : base("name=DBMEIMModel")
        {
        }

        public virtual DbSet<ConfigDatabase> ConfigDatabases { get; set; }
        public virtual DbSet<ESOrganization> ESOrganizations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConfigDatabase>().Property(e => e.ID);
            modelBuilder.Entity<ESOrganization>().Property(e => e.Id);
        }
    }
}