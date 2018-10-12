﻿using Microsoft.EntityFrameworkCore;


namespace pindogramApp.Entities
{
    public class PindogramDataContext : DbContext
    {
        public PindogramDataContext(DbContextOptions<PindogramDataContext> options) : base(options)
        {

        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }
    }
}