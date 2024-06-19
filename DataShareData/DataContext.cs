using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using Pomelo.EntityFrameworkCore.MySql.Design;
namespace DataShareData;

using DataShareCore.Models;
 public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>(
                    e =>
                    {
                        e.ToTable("Account");
                        e.HasKey(x => new { x.id });// set primary key
                    }); 
           
        }

        #region DbSet
        
        public DbSet<Account> Account { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<FileStore> FileStores { get; set; }
        
        #endregion
        
    }