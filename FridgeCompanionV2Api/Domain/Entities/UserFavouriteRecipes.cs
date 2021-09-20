namespace FridgeCompanionV2Api.Domain.Entities
{
    public class UserFavouriteRecipes
    {
        public int Id { get; set; }
        public virtual Recipe Recipe { get; set; }
        public virtual User User { get; set; }
    }
}