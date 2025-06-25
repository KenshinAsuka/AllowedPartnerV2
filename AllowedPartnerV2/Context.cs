using AllowedPartnerV2.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace AllowedPartnerV2
{
    public class Context : DbContext
    {
    
        public DbSet<Items> Items { get; set; }
        public DbSet<Partner> Partners { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //    modelBuilder.Entity<Partner>()
        //   .HasKey(p => p.partnerkey);

        //    modelBuilder.Entity<Items>()
        //        .HasKey(i => i.partneritemref);

        //    modelBuilder.Entity<Items>()
        //        .HasOne(i => i.Partner)
        //        .WithMany(p => p.Items)
        //        .HasForeignKey(p.); 
        //}
    }
}
