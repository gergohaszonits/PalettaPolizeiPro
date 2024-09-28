using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.EKS;
using PalettaPolizeiPro.Data.LineEvents;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Data.Users;

namespace PalettaPolizeiPro.Database
{
    public class DatabaseContext : DbContext
    {
        private static string? _connectionString { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Paletta> Palettas { get; set; }
        public DbSet<PalettaProperty> PalettaProperties { get; set; }
        public DbSet<ServerNotification> ServerNotifications { get; set; }
        public DbSet<QueryEventArgs> QueryEvents { get; set; }
        public DbSet<CheckEventArgs> CheckEvents { get; set; }
        public DbSet<EksEventArgs> EksEvents { get; set; }
        public DbSet<QueryState> QueryStates { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<OrderPalettaScheduled> OrderPalettaSchedules { get; set; }
        public DbSet<OrderPalettaFinished> OrderPalettaFinishes { get; set; }
        public DbSet<Eks> Eks { get; set; }

        public DatabaseContext() : base()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString is null) { throw new Exception("Connection string is empty..."); }
            optionsBuilder.EnableSensitiveDataLogging();
            if (_connectionString.Contains("Host="))
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paletta>()
             .HasMany(e => e.InScheduled)
             .WithMany(e => e.ScheduledPalettas)
             .UsingEntity<OrderPalettaScheduled>();

            modelBuilder.Entity<Paletta>()
            .HasMany(e => e.InFinished)
            .WithMany(e => e.FinishedPalettas)
            .UsingEntity<OrderPalettaFinished>();

        }
        public static void Initialize(string connectionString)
        {
            _connectionString = connectionString;
            using (var context = new DatabaseContext())
            {
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex) { }
                int adminCount = context.Users.Where(x => x.Role == Role.Admin).Count();
                if (adminCount == 0)
                {
                    context.Users.Add(new User
                    {
                        Username = "sysadmin",
                        Password = HashString("ppadmin"),
                        Role = Role.Admin
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
