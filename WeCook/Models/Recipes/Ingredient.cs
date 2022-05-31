using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeCook.Models.Recipes
{
    public class Ingredient
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Champ requis")]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Champ requis")]
        [Range(0.1, float.MaxValue, ErrorMessage = "Nombre positif requis")]
        [Display(Name = "Quantité")]
        public float Quantity { get; set; }

        [Required(AllowEmptyStrings = true)]
        [Display(Name = "Unité")]
        public IngredientUnit Unit { get; set; }


        [ValidateNever]
        public Recipe Recipe { get; set; }
        [ValidateNever]
        public int RecipeId { get; set; }
    
    
        [NotMapped]
        public string DisplayUnit
        {
            get {
                Dictionary<IngredientUnit, string> units = new Dictionary<IngredientUnit, string>()
                {
                    { IngredientUnit.None, ""},
                    { IngredientUnit.Unit, ""},
                    { IngredientUnit.Gram, "G"},
                    { IngredientUnit.Kilogram, "Kg"},
                    { IngredientUnit.Milliliter, "Ml"},
                    { IngredientUnit.Centiliter, "Cl"},
                    { IngredientUnit.Liter, "L"},
                    { IngredientUnit.Teaspoon, " Càc"},
                    { IngredientUnit.Tablespoon, " Càs"},
                    { IngredientUnit.Pinch, " Pincée"},
                };

                return units[Unit];
            }
            set { _displayUnit = value; }
        } 
        private string _displayUnit;
    }
}
