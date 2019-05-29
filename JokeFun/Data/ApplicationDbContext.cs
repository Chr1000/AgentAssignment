using System;
using System.Collections.Generic;
using System.Text;
using JokeFun.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JokeFun.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Joke> Jokes { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
