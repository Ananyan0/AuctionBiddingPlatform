using AuctionBiddingPlatform.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AuctionBiddingPlatform.Infrastructure.Data;

public class AuctionDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public AuctionDbContext(DbContextOptions<AuctionDbContext> options)
        : base(options) { }

    public DbSet<AuctionItem> AuctionItems { get; set; } = default!;
    public DbSet<Bid> Bids { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // AuctionItem configuration
        builder.Entity<AuctionItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.StartingPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.HighestBid).HasColumnType("decimal(18,2)");
            entity.Property(e => e.StartsAtUtc).IsRequired();
            entity.Property(e => e.EndsAtUtc).IsRequired();
        });

        // Bid configuration
        builder.Entity<Bid>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PlacedAtUtc)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Bids)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AuctionItem)
                  .WithMany(a => a.Bids)
                  .HasForeignKey(e => e.AuctionItemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}