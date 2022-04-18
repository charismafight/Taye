using Microsoft.EntityFrameworkCore;
using Taye.Enums;
using Taye.Models;

namespace Taye.Repositories
{
    public class TayeContext : DbContext
    {
        public TayeContext(DbContextOptions<TayeContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Archive>()
                .Property(p => p.PubDate)
                .HasDefaultValueSql("now()");

            //上传文件的枚举类型和上传时间的默认值处理
            modelBuilder.Entity<UploadFile>()
                .Property(p => p.UploadTime)
                .HasDefaultValueSql("now()");

            modelBuilder.Entity<UploadFile>()
                .Property(p => p.FileType)
                .HasConversion(
                o => o.ToString(),
                o => (FileType)Enum.Parse(typeof(FileType), o));
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Archive> Archives { get; set; }
        public virtual DbSet<UploadFile> UploadFiles { get; set; }
    }
}
