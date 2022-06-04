using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using WeCook.Models.Recipes;

namespace WeCook.Models.Users
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public DateTime BirthDate { get; set; }

        public DateTime RegisteredAt { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [ValidateNever]
        public string AvatarName { get; set; }

        public ICollection<Recipe> Recipes { get; set; }

        public ICollection<UserRecipe> LikedRecipes { get; set; }

    }
}
