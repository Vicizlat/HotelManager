using System;
using HotelManager.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManager.Data
{
    public class HotelManagerContext : DbContext
    {
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public string ConnectionString { get; set; }

        public HotelManagerContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public HotelManagerContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                MariaDbServerVersion mariaDbServer = new MariaDbServerVersion(new Version(5, 5, 57));
                optionsBuilder.UseMySql(ConnectionString, mariaDbServer);
                //optionsBuilder.UseSqlServer(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Transaction>().HasKey(x => new { x.GuestId, x.ReservationId });
            // Solve cyclic dependency ON DELETE....
            modelBuilder.Entity<Transaction>()
                .HasOne(x => x.Guest)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.GuestId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Transaction>()
                .HasOne(x => x.Reservation)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Guest>()
                .HasOne(g => g.GuestReferrer)
                .WithMany(g => g.GuestReferrals)
                .HasForeignKey(g => g.GuestReferrerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}