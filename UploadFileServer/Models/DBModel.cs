namespace UploadFileServer.Models
{
    using System.Data.Entity;

    public partial class DBModel : DbContext
    {
        public DBModel()
            : base("name=DBModel")
        {
        }

        public virtual DbSet<DocumentOutAttachment> DocumentOutAttachments { get; set; }
        public virtual DbSet<DocumentOut> DocumentOuts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentOut>()
                .Property(e => e.DocumentTypeID)
                .IsUnicode(false);
        }
    }
}
