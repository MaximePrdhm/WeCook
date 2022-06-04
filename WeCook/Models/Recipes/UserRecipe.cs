using WeCook.Models.Users;

namespace WeCook.Models.Recipes
{
    public class UserRecipe
    {
        public string UserId { get; set; }
        public User User { get; set; }
    
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
