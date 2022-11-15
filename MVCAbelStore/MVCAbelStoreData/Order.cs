using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MVCAbelStoreData
{
    public enum OrderStatus
    {
        New, Shipped, Cancelled, Void
    }
    public class Order : EntityBase
    {
        public string? DeliveryCode { get; set; }

        public OrderStatus Status { get; set; }

        public Guid ApplicationUserId { get; set; }

        public Guid DeliveryAddressId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual Address DeliveryAddress { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

    }
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder
                .HasMany(p => p.OrderItems)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
