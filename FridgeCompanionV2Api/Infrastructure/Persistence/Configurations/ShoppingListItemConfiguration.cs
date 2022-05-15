using FridgeCompanionV2Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FridgeCompanionV2Api.Infrastructure.Persistence.Configurations
{
    public class ShoppingListItemConfiguration : IEntityTypeConfiguration<ShoppingListItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingListItem> builder)
        {
        }
    }
}