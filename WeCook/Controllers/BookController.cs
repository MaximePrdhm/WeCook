using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WeCook.Models.Database;
using WeCook.Models.ViewModel.RecipeBook;

namespace WeCook.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly WeCookDbContext _context;

        public BookController(WeCookDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return RedirectToAction("Me");
        }

        public IActionResult Me()
        {
            var logon = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var created = _context.Recipes
                .Include(r => r.Creator)
                .Where(r => r.CreatorId == logon)
                .ToList();
            var liked = _context.Recipes
                .Include(r => r.Likes)
                .ThenInclude(l => l.User)
                .Where(r => r.Likes.Contains(new Models.Recipes.UserRecipe { RecipeId=r.Id, UserId=logon}))
                .ToList();

            return View(new UserRecipeBookViewModel()
            {
                LikedRecipes = liked,
                CreatedRecipes = created
            }); ;
        }
    }
}
