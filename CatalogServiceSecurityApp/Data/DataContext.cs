﻿namespace CatalogServiceSecurityApp.Data
{
    using CatalogServiceSecurityApp.Models.DbModels;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection.Emit;

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<UserEvent> UserEvents { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>(entity =>
            {
                entity.HasKey(e => new { e.Id });
            });

            builder.Entity<User>()
                .HasKey(u => new { u.UserId });

            builder.Entity<Event>()
                .HasKey(e => new { e.Id });

            builder.Entity<UserEvent>()
                .HasKey(ue => new { ue.UserId, ue.EventId });

            builder.Entity<UserEvent>()
                .HasOne(ue => ue.User)
                .WithMany(user => user.UserEvents)
                .HasForeignKey(u => u.UserId);

            builder.Entity<UserEvent>()
                .HasOne(uc => uc.Event)
                .WithMany(ev => ev.UserEvents)
                .HasForeignKey(ev => ev.EventId);
        }
    }
}