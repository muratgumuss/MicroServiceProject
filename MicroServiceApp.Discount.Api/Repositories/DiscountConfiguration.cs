using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace MicroServiceApp.Discount.Api.Repositories
{

    public class DiscountConfiguration : IEntityTypeConfiguration<Features.Discounts.Discount>
    {
        public void Configure(EntityTypeBuilder<Features.Discounts.Discount> builder)
        {
            builder.ToCollection("discounts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasElementName("_id");
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Code).HasElementName("code").HasMaxLength(10);
            builder.Property(x => x.Rate).HasElementName("rate");
            builder.Property(x => x.UserId).HasElementName("user_id");
            builder.Property(x => x.Created).HasElementName("created");
            builder.Property(x => x.Updated).HasElementName("updated");
            builder.Property(x => x.Expired).HasElementName("expired");
        }
    }
}
