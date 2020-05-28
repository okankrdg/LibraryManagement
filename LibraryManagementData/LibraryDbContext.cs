using LibraryManagementData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace LibraryManagementData
{
    public class LibraryDbContext:DbContext
    {
        public LibraryDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Patron> Patrons { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Checkout> Checkouts { get; set; }
        public DbSet<CheckoutHistory> CheckoutHistories { get; set; }
        public DbSet<LibraryBranch> LibraryBranches { get; set; }
        public DbSet<BranchHours> BranchHours { get; set; }
        public DbSet<LibraryCard> LibraryCards { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<LibraryAsset> LibraryAssets { get; set; }
        public DbSet<Hold> Holds { get; set; }
        public DbSet<Book> Books { get; set; }

    }
    //https://www.gencayyildiz.com/blog/net-core-class-libraryde-migration-islemleri/
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
    {
        public LibraryDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<LibraryDbContext>();
            var connectionString = "server=.;database=Library_Dev;uid=sa;pwd=123;MultipleActiveResultSets=true";
            builder.UseSqlServer(connectionString);
            return new LibraryDbContext(builder.Options);
        }
    }
}
