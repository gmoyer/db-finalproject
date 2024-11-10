using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Models;

namespace ServerSubscriptionManager.Context
{
    public class SubscriptionContext : DbContext
    {
        public required DbSet<User> Users { get; set; }
        public required DbSet<SubscriptionPeriod> SubscriptionPeriods { get; set; }
        public required DbSet<PaymentMethod> PaymentMethods { get; set; }
        public required DbSet<Payment> Payments { get; set; }
        public required DbSet<Invoice> Invoices { get; set; }

        public string DbPath { get; }

        public SubscriptionContext(DbContextOptions options) : base(options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "subscription.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=ServerSubscriptionManager.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .Property(e => e.Date)
                .HasDefaultValueSql("CURRENT_DATE");
        }
    }
}
