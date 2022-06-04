using WeCook.Models.Recipes;
using WeCook.Models.Users;

namespace WeCook.Models.ViewModel.RecipeBook
{
    public class UserRecipeBookViewModel
    {
        public User User { get; set; }
        public List<Recipe> CreatedRecipes { get; set; }
        public List<Recipe> LikedRecipes { get; set; }
    }
}
