using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using EventManager.Server.Models.EventManagerDb;

namespace EventManager.Server.Data
{
    public partial class EventManagerDbContext : DbContext
    {
        public EventManagerDbContext()
        {
        }

        public EventManagerDbContext(DbContextOptions<EventManagerDbContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EventManager.Server.Models.EventManagerDb.Event>()
              .Property(p => p.Date)
              .HasColumnType("datetimeoffset");
            this.OnModelBuilding(builder);
        }

        public DbSet<EventManager.Server.Models.EventManagerDb.Attendee> Attendees { get; set; }

        public DbSet<EventManager.Server.Models.EventManagerDb.Event> Events { get; set; }

        public DbSet<EventManager.Server.Models.EventManagerDb.EventAttendee> EventAttendees { get; set; }

    }
}