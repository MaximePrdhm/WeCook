using System.ComponentModel.DataAnnotations;

namespace WeCook.Models.Recipes
{
    public enum RecipeType
    {
        [Display(Name = "Amuse-Bouche")]
        Appetizer,
        [Display(Name = "Entrée")]
        Starter,
        [Display(Name = "Plat Principal")]
        MainDish,
        Dessert,
        [Display(Name = "Boisson")]
        Drink,
        [Display(Name = "Pâtisserie")]
        Bakery
    }
}
