using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Database.Models
{
    public partial class QLTTrContext : DbContext
    {
        public QLTTrContext()
        {
        }

        public QLTTrContext(DbContextOptions<QLTTrContext> options)
            : base(options)
        {
        }
        public virtual DbSet<AppUser> AppUsers { get; set; } = null;
        public virtual DbSet<AppRole> AppRoles { get; set; } = null;
        public virtual DbSet<AppUserInRole> AppUsersInRoles { get; set; } = null;        
        public virtual DbSet<TblComment> TblComments { get; set; } = null!;
        public virtual DbSet<TblDocument> TblDocuments { get; set; } = null!;
        public virtual DbSet<TblDocumentReview> TblDocumentReview { get; set; } = null!;
        public virtual DbSet<TblDocumentFile> TblDocumentFiles { get; set; } = null!;
        public virtual DbSet<TblField> TblFields { get; set; } = null!;
        public virtual DbSet<TblSetting> TblSettings { get; set; } = null!;
        public virtual DbSet<TblStatuses> TblStatuses { get; set; } = null!;
        public virtual DbSet<TblNotification> TblNotifications { get; set; } = null!;
        public virtual DbSet<TblSMSLog> TblSMSLogs { get; set; } = null!;
        public virtual DbSet<TblDocumentHistory> TblDocumentHistories { get; set; } = null!;
        public virtual DbSet<TblDocumentTypes> TblDocumentTypes { get; set; } = null!;
        public virtual DbSet<CfgWorkFlow> CfgWorkFlow { get; set; } = null!;
        public virtual DbSet<CfgWorkFlowGroup> CfgWorkFlowGroup { get; set; } = null!;
        public virtual DbSet<CfgWorkFlowUser> ReviewOrderUserDetail { get; set; } = null!;
        public virtual DbSet<TblDeparments> TblDeparments { get; set; } = null!;
        public virtual DbSet<TblGroups> TblGroups { get; set; } = null!;
        public virtual DbSet<TblGroupDetails> TblGroupDetails { get; set; } = null!;
        public virtual DbSet<TblUnit> TblUnits { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-PCHHO158\\TESTDB;Database=QLTTr;Trusted_Connection=True;");
            }
        }
    }
}
