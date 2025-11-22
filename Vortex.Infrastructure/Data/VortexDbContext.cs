using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vortex.Domain;
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
            });

            builder.Entity<AttachmentEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("tbl_attachment_master");
            });

            // Identity tables
            builder.Entity<UserEntity>().ToTable("tbl_user_master");
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