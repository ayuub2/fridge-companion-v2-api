using FridgeCompanionV2Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace FridgeCompanionV2Api.Infrastructure.Persistence.Configurations
{
    public class ShoppingListRecipeConfiguration : IEntityTypeConfiguration<ShoppingListRecipeItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingListRecipeItem> builder)
        {
            builder.HasQueryFilter(item => !item.IsDeleted);
        }
    }
}