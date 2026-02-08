using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vortex.Domain.Constants;
using Vortex.Domain.Entities;

namespace Vortex.Infrastructure.Data
{
    public class VortexDbContext(DbContextOptions<VortexDbContext> options)
        : IdentityDbContext<UserEntity, RoleEntity, Guid>(options)
    {
        public DbSet<ProjectEntity> Projects { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<UserProjectRole>  UserProjectRoles { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<AttachmentEntity> Attachments { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TaskEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("tbl_task_master");
            });

            builder.Entity<ProjectEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("tbl_project_master");
                entity.HasData(new ProjectEntity
                {
                    Id = Constants.DefaultProjectId,
                    ProjectName = "Default",
                    ProjectKey = "Default",
                    Description = "This is a default project",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = Guid.Empty,
                    UpdatedBy = Guid.Empty
                });
            });

            builder.Entity<AttachmentEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("tbl_attachment_master");
            });

            // Identity tables
            builder.Entity<UserEntity>().ToTable("tbl_user_master");
            // Explicit FK mapping between User and Role
            builder.Entity<UserEntity>().HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);
            builder.Entity<RoleEntity>().ToTable("tbl_role_master").HasData([
                new RoleEntity
                {
                    Id = Constants.AdminRoleId,
                    Name = "Admin",
                },
                new RoleEntity
                {
                    Id = Constants.ManagerRoleId,
                    Name = "Manager",
                },
                new RoleEntity
                {
                    Id = Constants.MemberRoleId,
                    Name = "Member",
                }
                
            ]);
            
            // other
            builder.Entity<UserProjectRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("tbl_user_project_master");
            });
            builder.Entity<AttachmentEntity>().ToTable("tbl_attachment_master");
        }
        
    }
}