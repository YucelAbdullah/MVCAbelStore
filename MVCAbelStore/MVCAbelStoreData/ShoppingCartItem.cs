using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MVCAbelStoreData
{
    public class ShoppingCartItem : EntityBase
    {
        public Guid ProductId { get; set; }

        public Guid ApplicationUserId { get; set; }

        public int Quantity { get; set; }

        public virtual Product? Product { get; set; }


        public virtual ApplicationUser? ApplicationUser { get; set; }
    }

    public class ShoppingCartEntityTypeConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {

        }
    }
}
