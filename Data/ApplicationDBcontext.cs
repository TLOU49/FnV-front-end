using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
  public class ApplicationDBcontext : IdentityDbContext<AppUser>
  {
    public ApplicationDBcontext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {

    }

    public DbSet<Products> Products { get; set; }
    public DbSet<Basket> Baskets { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      // builder.Entity<Products>();
      // builder.Entity<Basket>();

      List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                }
            };
      builder.Entity<IdentityRole>().HasData(roles);
      builder.Entity<AppUser>()
      .HasIndex(u => u.Email)
      .IsUnique();
    }
  }
}