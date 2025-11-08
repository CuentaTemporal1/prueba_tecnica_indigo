using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Prueba.Domain.Entities;

namespace Prueba.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
         
            builder.HasOne(o => o.User)
                   .WithMany()
                   .HasForeignKey(o => o.UserId);

            builder.Property(o => o.TotalAmount)
                   .HasColumnType("decimal(18,2)");
        }
    }
}