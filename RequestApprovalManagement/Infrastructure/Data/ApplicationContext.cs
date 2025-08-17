using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;
using System.Reflection.Emit;

namespace Infrastructure.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        
    }

    public virtual DbSet<AppUser> AppUsers { get; set; } = null;
    public virtual DbSet<AppRole> AppRoles { get; set; } = null;
    public virtual DbSet<AppUserInRole> AppUsersInRoles { get; set; } = null;
    public virtual DbSet<TblComment> TblComments { get; set; } = null!;
    public virtual DbSet<TblDocument> TblDocuments { get; set; } = null!;
    public virtual DbSet<TblDocumentApproval> TblDocumentApprovals { get; set; } = null!;
    public virtual DbSet<TblDocumentFile> TblDocumentFiles { get; set; } = null!;
    public virtual DbSet<TblField> TblFields { get; set; } = null!;
    public virtual DbSet<TblSetting> TblSettings { get; set; } = null!;
    public virtual DbSet<TblStatus> TblStatuses { get; set; } = null!;
    public virtual DbSet<TblNotification> TblNotifications { get; set; } = null!;
    public virtual DbSet<TblSMSLog> TblSMSLogs { get; set; } = null!;
    public virtual DbSet<TblDocumentHistory> TblDocumentHistories { get; set; } = null!;
    public virtual DbSet<CategoryStatusesEntity> CategoryStatusesEntity { get; set; } = null!;
    public virtual DbSet<ReviewOrderEntity> ReviewOrderEntity { get; set; } = null!;
    public virtual DbSet<ReviewOrderGroupDetailEntity> ReviewOrderGroupDetailEntity { get; set; } = null!;
    public virtual DbSet<ReviewOrderUserDetailEntity> ReviewOrderUserDetailEntity { get; set; } = null!;

    public virtual DbSet<CategoryDocumentTypesEntity> CategoryDocumentTypesEntity { get; set; } = null!;
    public virtual DbSet<CategoryFieldsEntity> CategoryFieldsEntity { get; set; } = null!;

    public virtual DbSet<ReviewUserDetailEntity> ReviewUserDetailEntity { get; set; } = null!;
    public virtual DbSet<ReviewProcessEntity> ReviewProcessEntity { get; set; } = null!;
    public virtual DbSet<ReviewProcessDetailEntity> ReviewProcessDetailEntity { get; set; } = null!;



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
