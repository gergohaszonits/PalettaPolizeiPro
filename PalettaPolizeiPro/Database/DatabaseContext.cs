﻿using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.Palettas;

namespace PalettaPolizeiPro.Database
{
    public class DatabaseContext : DbContext
    {
        private static string? _connectionString {  get; set; }   
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Paletta> Palettas { get; set; }
        public DbSet<PalettaProperty> PalettaProperties { get; set; }
        public DbSet<QueryNotification> QueryNotifications { get; set; }
        public DbSet<PalettaNotification> PalettaNotifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }


        public DatabaseContext() : base()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString is null) { throw new Exception("Connection string is empty..."); }
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
             .WithMany(e => e.ScheduledPalettas);

            modelBuilder.Entity<Paletta>()
            .HasMany(e => e.InFinished)
            .WithMany(e => e.FinishedPalettas);
        }
        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
