using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeCook.Models.Recipes
{
    public class Recipe : IValidatableObject
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Champ requis")]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        [Display(Name = "Nom")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Champ requis")]
        [Display(Name = "Type de Plat")]
        public RecipeType Type { get; set; }

        [Required(ErrorMessage = "Champ requis")]
        [Range(1,5, ErrorMessage = "Valeurs : 1-5")]
        [Display(Name = "Difficulté")]
        public int Difficulty { get; set; }

        [DefaultValue(0)]
        [Required(ErrorMessage = "Champ requis")]
        [Range(0,int.MaxValue, ErrorMessage = "Nombre positif requis")]
        [Display(Name = "Préparation")]
        public int PreparationTime { get; set; }

        [DefaultValue(0)]
        [Required(ErrorMessage = "Champ requis")]
        [Range(0, int.MaxValue, ErrorMessage = "Nombre positif requis")]
        [Display(Name = "Cuisson")]
        public int CookingTime { get; set; }
        
        [NotMapped]
        public int Duration { get; private set; }

        [Required(ErrorMessage = "Champ requis")]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum : 1 Pers.")]
        [Display(Name = "Personnes")]
        public int PeopleFor { get; set; }

        [Required(ErrorMessage = "Champ requis")]
        [NotMapped]
        [Display(Name = "Image de la recette")]
        public IFormFile ImageFile { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [ValidateNever]
        public string ImageName { get; set; }


        public IList<Step> Steps { get; set; } = new List<Step>();

        public IList<Ingredient> Ingredients { get; set; } = new List<Ingredient>();


        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if(PreparationTime + CookingTime == 0)
            {
                yield return new ValidationResult("Temps total nul non accepté", new List<string>() { nameof(PreparationTime), nameof(CookingTime) });
            }
        }
    }
}
