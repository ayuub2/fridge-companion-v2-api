using FridgeCompanionV2Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<TodoList> TodoLists { get; set; }

        DbSet<TodoItem> TodoItems { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<ShoppingList> ShoppingLists { get; set; }
        DbSet<ShoppingListItem> ShoppingListItems { get; set; }
    }
}
