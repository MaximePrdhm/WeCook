using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WeCook.Models.Recipes
{
    public class Step
    {
        public int Id { get; set; }

        [ValidateNever]
        public int Position { get; set; }

        [Required(ErrorMessage = "Champ requis")]
        public string Description { get; set; }

        [ValidateNever]
        public Recipe Recipe { get; set; }
        [ValidateNever]
        public int RecipeId { get; set; }
    }
}
