using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Database.Models.Website;

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

        // ===== CÁC DBSET CŨ (QLTTr) =====
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

        // ===== CÁC DBSET MỚI (WEBSITE VIỆC LÀM) =====
        
        // Core Entities
        public virtual DbSet<Worker> Workers { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<JobPosting> JobPostings { get; set; }
        public virtual DbSet<JobApplication> JobApplications { get; set; }
        public virtual DbSet<CV> CVs { get; set; }
        public virtual DbSet<Interview> Interviews { get; set; }

        // Worker Related
        public virtual DbSet<WorkerSkill> WorkerSkills { get; set; }
        public virtual DbSet<WorkerExperience> WorkerExperiences { get; set; }
        public virtual DbSet<WorkerEducation> WorkerEducations { get; set; }
        public virtual DbSet<WorkerDocument> WorkerDocuments { get; set; }

        // Company Related
        public virtual DbSet<CompanyDocument> CompanyDocuments { get; set; }
        public virtual DbSet<BusinessApproval> BusinessApprovals { get; set; }
        public virtual DbSet<VerificationDocument> VerificationDocuments { get; set; }

        // Job Related
        public virtual DbSet<JobRequirement> JobRequirements { get; set; }
        public virtual DbSet<JobBenefit> JobBenefits { get; set; }

        // CV Related
        public virtual DbSet<CVSection> CVSections { get; set; }

        // Interview Related
        public virtual DbSet<InterviewResult> InterviewResults { get; set; }

        // User Management
        public virtual DbSet<SavedJob> SavedJobs { get; set; }
        public virtual DbSet<SearchHistory> SearchHistory { get; set; }
        public virtual DbSet<DashboardStats> DashboardStats { get; set; }
        public virtual DbSet<UserActivity> UserActivity { get; set; }

        // Communication
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<EmailLog> EmailLogs { get; set; }

        // System Logging
        public virtual DbSet<SystemLog> SystemLogs { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }

        // Category Entities
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Commune> Communes { get; set; }
        public virtual DbSet<EducationLevel> EducationLevels { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<CareerGroup> CareerGroups { get; set; }
        public virtual DbSet<Specialization> Specializations { get; set; }
        public virtual DbSet<Career> Careers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-PCHHO158\\TESTDB;Database=QLTTr;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình các relationship và constraints cho website việc làm
            ConfigureWebsiteRelationships(modelBuilder);
        }

        private void ConfigureWebsiteRelationships(ModelBuilder modelBuilder)
        {
            // AppUser Relationships
            modelBuilder.Entity<AppUser>()
                .HasOne(au => au.Worker)
                .WithOne(w => w.AppUser)
                .HasForeignKey<Worker>(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppUser>()
                .HasOne(au => au.Company)
                .WithOne(c => c.AppUser)
                .HasForeignKey<Company>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // AppUser -> Collections
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.AppUser)
                .WithMany(au => au.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SearchHistory>()
                .HasOne(sh => sh.AppUser)
                .WithMany(au => au.SearchHistory)
                .HasForeignKey(sh => sh.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DashboardStats>()
                .HasOne(ds => ds.AppUser)
                .WithMany(au => au.DashboardStats)
                .HasForeignKey(ds => ds.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserActivity>()
                .HasOne(ua => ua.AppUser)
                .WithMany(au => au.UserActivity)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.AppUser)
                .WithMany(au => au.AuditLogs)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SavedJob>()
                .HasOne(sj => sj.AppUser)
                .WithMany(au => au.SavedJobs)
                .HasForeignKey(sj => sj.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.AppUser)
                .WithMany(au => au.JobApplications)
                .HasForeignKey(ja => ja.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // AppRole Relationships
            modelBuilder.Entity<AppUserInRole>()
                .HasOne(auir => auir.AppUser)
                .WithMany(au => au.AppUsersInRoles)
                .HasForeignKey(auir => auir.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppUserInRole>()
                .HasOne(auir => auir.AppRole)
                .WithMany(ar => ar.AppUsersInRoles)
                .HasForeignKey(auir => auir.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Worker Relationships
            modelBuilder.Entity<Worker>()
                .HasOne(w => w.District)
                .WithMany(d => d.Workers)
                .HasForeignKey(w => w.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Worker>()
                .HasOne(w => w.Commune)
                .WithMany(c => c.Workers)
                .HasForeignKey(w => w.CommuneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Worker>()
                .HasOne(w => w.EducationLevel)
                .WithMany(e => e.Workers)
                .HasForeignKey(w => w.EducationLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Worker>()
                .HasOne(w => w.Career)
                .WithMany(c => c.Workers)
                .HasForeignKey(w => w.CareerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Worker Related Collections
            modelBuilder.Entity<WorkerSkill>()
                .HasOne(ws => ws.Worker)
                .WithMany(w => w.Skills)
                .HasForeignKey(ws => ws.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkerExperience>()
                .HasOne(we => we.Worker)
                .WithMany(w => w.Experiences)
                .HasForeignKey(we => we.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkerEducation>()
                .HasOne(we => we.Worker)
                .WithMany(w => w.Educations)
                .HasForeignKey(we => we.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkerDocument>()
                .HasOne(wd => wd.Worker)
                .WithMany(w => w.Documents)
                .HasForeignKey(wd => wd.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Company Relationships
            modelBuilder.Entity<Company>()
                .HasOne(c => c.District)
                .WithMany(d => d.Companies)
                .HasForeignKey(c => c.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.Commune)
                .WithMany(com => com.Companies)
                .HasForeignKey(c => c.CommuneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CompanyDocument>()
                .HasOne(cd => cd.Company)
                .WithMany(c => c.Documents)
                .HasForeignKey(cd => cd.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BusinessApproval>()
                .HasOne(ba => ba.Company)
                .WithMany(c => c.BusinessApprovals)
                .HasForeignKey(ba => ba.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Job Relationships
            modelBuilder.Entity<JobPosting>()
                .HasOne(j => j.Company)
                .WithMany(c => c.JobPostings)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobPosting>()
                .HasOne(j => j.Field)
                .WithMany(f => f.JobPostings)
                .HasForeignKey(j => j.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobPosting>()
                .HasOne(j => j.Career)
                .WithMany(c => c.JobPostings)
                .HasForeignKey(j => j.CareerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobPosting>()
                .HasOne(j => j.District)
                .WithMany(d => d.JobPostings)
                .HasForeignKey(j => j.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobRequirement>()
                .HasOne(jr => jr.JobPosting)
                .WithMany(j => j.JobRequirements)
                .HasForeignKey(jr => jr.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobBenefit>()
                .HasOne(jb => jb.JobPosting)
                .WithMany(j => j.JobBenefits)
                .HasForeignKey(jb => jb.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // JobApplication Relationships
            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.JobPosting)
                .WithMany(j => j.JobApplications)
                .HasForeignKey(ja => ja.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Worker)
                .WithMany(w => w.JobApplications)
                .HasForeignKey(ja => ja.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Interview Relationships
            modelBuilder.Entity<Interview>()
                .HasOne(i => i.JobApplication)
                .WithOne(ja => ja.Interview)
                .HasForeignKey<Interview>(i => i.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InterviewResult>()
                .HasOne(ir => ir.Interview)
                .WithOne(i => i.InterviewResult)
                .HasForeignKey<InterviewResult>(ir => ir.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // CV Relationships
            modelBuilder.Entity<CV>()
                .HasOne(cv => cv.Worker)
                .WithMany(w => w.CVs)
                .HasForeignKey(cv => cv.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CVSection>()
                .HasOne(cvs => cvs.CV)
                .WithMany(cv => cv.Sections)
                .HasForeignKey(cvs => cvs.CVId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category Relationships
            modelBuilder.Entity<Commune>()
                .HasOne(c => c.District)
                .WithMany(d => d.Communes)
                .HasForeignKey(c => c.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CareerGroup>()
                .HasOne(cg => cg.Field)
                .WithMany(f => f.CareerGroups)
                .HasForeignKey(cg => cg.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Specialization>()
                .HasOne(s => s.CareerGroup)
                .WithMany(cg => cg.Specializations)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Career>()
                .HasOne(c => c.Specialization)
                .WithMany(s => s.Careers)
                .HasForeignKey(c => c.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkerEducation>()
                .HasOne(we => we.EducationLevel)
                .WithMany(el => el.WorkerEducations)
                .HasForeignKey(we => we.EducationLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            // User Management Relationships
            modelBuilder.Entity<SavedJob>()
                .HasOne(sj => sj.Worker)
                .WithMany(w => w.SavedJobs)
                .HasForeignKey(sj => sj.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavedJob>()
                .HasOne(sj => sj.JobPosting)
                .WithMany(j => j.SavedJobs)
                .HasForeignKey(sj => sj.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // Communication Relationships
            modelBuilder.Entity<EmailLog>()
                .HasOne(el => el.EmailTemplate)
                .WithMany(et => et.EmailLogs)
                .HasForeignKey(el => el.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
