using System.ComponentModel.DataAnnotations;

namespace WeCook.Models.Recipes
{
    public enum IngredientUnit
    {
        [Display(Name = "")]
        None,
        [Display(Name = "Unité")]
        Unit,
        [Display(Name = "Gramme")]
        Gram,
        [Display(Name = "Kilogramme")]
        Kilogram,
        [Display(Name = "Millilitre")]
        Milliliter,
        [Display(Name = "Centilitre")]
        Centiliter,
        [Display(Name = "Litre")]
        Liter,
        [Display(Name = "C. à Soupe")]
        Tablespoon,
        [Display(Name = "C. à Café")]
        Teaspoon,
        [Display(Name = "Pincée")]
        Pinch
    }
}
