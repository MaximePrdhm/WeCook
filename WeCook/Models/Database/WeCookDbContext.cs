using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeCook.Models.Recipes;
using WeCook.Models.Users;

namespace WeCook.Models.Database
{
    public class WeCookDbContext : IdentityDbContext<User> 
    {
        public WeCookDbContext(DbContextOptions<WeCookDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Ingredient>().Property(i => i.Unit).HasConversion(v => v.ToString(), v => (IngredientUnit)Enum.Parse(typeof(IngredientUnit), v));

            builder.Entity<Recipe>().Property(r => r.Duration).HasComputedColumnSql($"{nameof(Recipe.PreparationTime)} + {nameof(Recipe.CookingTime)}");
            builder.Entity<Recipe>().Property(r => r.Type).HasConversion(v => v.ToString(), v => (RecipeType)Enum.Parse(typeof(RecipeType), v));
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Step> Steps { get; set; }
    }
}
