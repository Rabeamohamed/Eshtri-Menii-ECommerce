using ECom.Core.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Infrastructure.Data.Config
{
    public class OrderItemsConfiguration : IEntityTypeConfiguration<OrderItems>
    {
        public void Configure(EntityTypeBuilder<OrderItems> builder)
        {
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
        }
    }
}
