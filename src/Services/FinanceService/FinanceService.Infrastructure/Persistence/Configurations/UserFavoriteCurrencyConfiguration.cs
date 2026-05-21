using FinanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceService.Infrastructure.Persistence.Configurations;

internal sealed class UserFavoriteCurrencyConfiguration : IEntityTypeConfiguration<UserFavoriteCurrency>
{
    public void Configure(EntityTypeBuilder<UserFavoriteCurrency> builder)
    {
        builder.ToTable("user_favorites");
        builder.HasKey(f => new { f.UserId, f.CurrencyId });

        builder.Property(f => f.UserId).HasColumnName("user_id");
        builder.Property(f => f.CurrencyId).HasColumnName("currency_id");

        builder.HasOne(f => f.Currency)
            .WithMany()
            .HasForeignKey(f => f.CurrencyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
