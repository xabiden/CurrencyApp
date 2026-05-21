using FinanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceService.Infrastructure.Persistence.Configurations;

internal sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    private static readonly Guid UsdId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid EurId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid RubId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("currency");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.Code)
            .HasColumnName("code")
            .HasMaxLength(10)
            .IsRequired();
        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(c => c.Rate)
            .HasColumnName("rate")
            .HasColumnType("numeric(18,6)");

        builder.HasIndex(c => c.Code).IsUnique();

        builder.HasData(
            new Currency(UsdId, "USD", "US Dollar", 90m),
            new Currency(EurId, "EUR", "Euro", 98m),
            new Currency(RubId, "RUB", "Russian Ruble", 1m));
    }
}